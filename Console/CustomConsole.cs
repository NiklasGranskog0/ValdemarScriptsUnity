#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.Scripts.Framework.Extensions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Framework.Console
{
    public class CustomConsole : EditorWindow
    {
        private Rect m_MessagePanel;
        private Vector2 m_MessageScroll;

        private Rect m_StacktracePanel;
        private Vector2 m_StacktraceScroll;
        private GUIStyle m_StacktraceStyle;

        private Rect m_MenuBar;

        private Rect m_Resizer;
        private bool m_IsResizing;
        private GUIStyle m_ResizeStyle;
        private float m_SizeRatio = 0.5f;

        private const float k_LogHeight = 40f;
        private const float k_ResizerHeight = 5f;
        private const float k_MenuBarHeight = 20f;

        private bool m_ErrorPause;
        private bool m_Collapse;
        private bool m_DisableUnityLogs;
        private bool m_ShowErrors = true;
        private bool m_ShowMessages = true;
        private bool m_ShowWarnings = true;

        public bool clearOnBuild = true;
        private bool m_ClearOnPlay = true;
        private bool m_ClearOnRecompile = true;

        private string m_SearchTag;
        private string m_SearchString;

        private int m_ErrorCount;
        private int m_MessageCount;
        private int m_WarningCount;

        private Texture2D m_ErrorIcon;
        private Texture2D m_ErrorIconSmall;

        private Texture2D m_WarningIcon;
        private Texture2D m_WarningIconSmall;

        private Texture2D m_MessageIcon;
        private Texture2D m_MessageIconSmall;

        private Texture2D m_ErrorIconSmallInactive;
        private Texture2D m_WarningIconSmallInactive;
        private Texture2D m_MessageIconSmallInactive;

        private GUIStyle m_MessageStyle;
        private Texture2D m_MessageBackgroundLight;
        private Texture2D m_MessageBackgroundDark;
        private Texture2D m_MessageSelected;
        private Texture2D m_LOGIcon;

        private Log m_SelectedLog;
        private List<Log> m_Logs = new();
        private readonly Color m_TextColor = new(0.7f, 0.7f, 0.7f);
        private static readonly StringBuilder s_stringBuilder = new();

        private static event Action<Log, LogType> OnReceivedData;

        private static readonly GUIContent s_clear = LogExtension.TextContent("Clear", "Clear console entries");
        private static readonly GUIContent s_clearOnPlay = LogExtension.TextContent("Clear on Play");
        private static readonly GUIContent s_clearOnBuild = LogExtension.TextContent("Clear on Build");
        private static readonly GUIContent s_clearOptions = LogExtension.TextContent("", "Clear Options");
        private static readonly GUIContent s_clearOnRecompile = LogExtension.TextContent("Clear on Recompile");
        private static readonly GUIContent s_unityLogs = LogExtension.TextContent("Unity", "Disable Unity Logs");

        private static readonly GUIContent s_errorPause =
            LogExtension.TextContent("Error Pause", "Pause Play Mode on error");

        private static readonly GUIContent s_collapse =
            LogExtension.TextContent("Collapse", "Collapse identical log messages");
        
        [MenuItem("Tools/Custom Console")]
        private static void OpenCustomConsole()
        {
            var type = Type.GetType("UnityEditor.ConsoleWindow, UnityEditor.dll");
            var window = GetWindow<CustomConsole>(type);
            window.titleContent = new GUIContent("Custom Console", UnityIcon.LoadIcon(UnityIcon.k_ConsoleIcon));
        }

        private void SubscribeToEvents(bool subscribe)
        {
            if (!subscribe)
            {
                OnReceivedData -= ReceivedData;
                Application.logMessageReceivedThreaded -= UnityLog;
                EditorApplication.playModeStateChanged -= OnPlayMode;
                AssemblyReloadEvents.afterAssemblyReload -= OnRecompile;
                ClearOnBuildClass.clearConsoleOnBuild -= OnBuild;
            }
            else
            {
                OnReceivedData += ReceivedData;
                Application.logMessageReceivedThreaded += UnityLog;
                EditorApplication.playModeStateChanged += OnPlayMode;
                AssemblyReloadEvents.afterAssemblyReload += OnRecompile;
                ClearOnBuildClass.clearConsoleOnBuild += OnBuild;
            }
        }

        private void Update() => Repaint();
        private void OnDestroy() => SubscribeToEvents(false);
        private void OnDisable() => SubscribeToEvents(false);

        private void OnEnable()
        {
            SubscribeToEvents(true);

            m_ResizeStyle = new GUIStyle
            {
                normal =
                {
                    background = UnityIcon.LoadIcon(UnityIcon.k_AvatarBlendBackground),
                }
            };

            m_MessageStyle = new GUIStyle
            {
                normal =
                {
                    textColor = m_TextColor
                },

                contentOffset = new Vector2(10f, 0f),
                alignment = TextAnchor.MiddleLeft
            };

            m_StacktraceStyle = new GUIStyle
            {
                normal =
                {
                    textColor = m_TextColor,
                    background = UnityIcon.LoadIcon(UnityIcon.k_StacktraceBackground),
                },
                wordWrap = true
            };

            m_ErrorIcon = UnityIcon.LoadIcon(UnityIcon.k_ErrorIcon);
            m_ErrorIconSmall = UnityIcon.LoadIcon(UnityIcon.k_ErrorIconSmall);
            m_ErrorIconSmallInactive = UnityIcon.LoadIcon(UnityIcon.k_ErrorIconSmallInactive);

            m_WarningIcon = UnityIcon.LoadIcon(UnityIcon.k_WarningIcon);
            m_WarningIconSmall = UnityIcon.LoadIcon(UnityIcon.k_WarningIconSmall);
            m_WarningIconSmallInactive = UnityIcon.LoadIcon(UnityIcon.k_WarningIconSmallInactive);

            m_MessageIcon = UnityIcon.LoadIcon(UnityIcon.k_MessageIcon);
            m_MessageIconSmall = UnityIcon.LoadIcon(UnityIcon.k_MessageIconSmall);
            m_MessageIconSmallInactive = UnityIcon.LoadIcon(UnityIcon.k_MessageIconSmallInactive);

            m_MessageBackgroundDark = UnityIcon.LoadIcon(UnityIcon.k_BackgroundOdd);
            m_MessageBackgroundLight = UnityIcon.LoadIcon(UnityIcon.k_BackgroundEven);
            m_MessageSelected = UnityIcon.LoadIcon(UnityIcon.k_BackgroundSelected);
        }

        private void OnGUI()
        {
            GUI.skin.horizontalScrollbar = GUIStyle.none;

            DrawMenuBar();
            DrawTopPanel();
            DrawResizer();
            DrawBottomPanel();

            ProcessEvents(Event.current);

            SetLogRect();

            if (m_SelectedLog != null)
            {
                if (m_SelectedLog.rect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    if (Event.current.clickCount == 2)
                    {
                        DoubleClickedLog();
                    }
                }
            }
        }

        private void DrawMenuBar()
        {
            m_MenuBar = new Rect(0, 0, position.width, k_MenuBarHeight);
            GUILayout.BeginArea(m_MenuBar, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            if (LogExtension.GUIButton(s_clear, 50f, EditorStyles.toolbarButton))
            {
                ClearConsole();
            }

            if (LogExtension.GUIButton(s_clearOptions, 16f, EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(s_clearOnPlay, m_ClearOnPlay, () => { m_ClearOnPlay = !m_ClearOnPlay; });
                menu.AddItem(s_clearOnBuild, clearOnBuild, () => { clearOnBuild = !clearOnBuild; });
                menu.AddItem(s_clearOnRecompile, m_ClearOnRecompile,
                    () => { m_ClearOnRecompile = !m_ClearOnRecompile; });
                menu.DropDown(new Rect(0f, 0f, 50f, k_MenuBarHeight));
            }

            m_DisableUnityLogs = LogExtension.GUIToggleButton(m_DisableUnityLogs, s_unityLogs, 40f);
            m_ErrorPause = LogExtension.GUIToggleButton(m_ErrorPause, s_errorPause, 70f);
            m_Collapse = LogExtension.GUIToggleButton(m_Collapse, s_collapse, 70f);

            GUI.SetNextControlName("m_SearchString");
            m_SearchString = LogExtension.GUITextField(m_SearchString, 250f);

            GUILayout.FlexibleSpace();

            LogExtension.GUILabel("Tag Search:");
            GUI.SetNextControlName("m_SearchTag");
            m_SearchTag = LogExtension.GUITextField(m_SearchTag, 100f);

            m_ShowMessages = LogExtension.GUIToggleButton(m_ShowMessages,
                new GUIContent(m_MessageCount <= 999 ? m_MessageCount.ToString() : "999+",
                    m_MessageCount == 0 ? m_MessageIconSmallInactive : m_MessageIconSmall),
                35f);

            m_ShowWarnings = LogExtension.GUIToggleButton(m_ShowWarnings,
                new GUIContent(m_WarningCount <= 999 ? m_WarningCount.ToString() : "999+",
                    m_WarningCount == 0 ? m_WarningIconSmallInactive : m_WarningIconSmall), 35f);

            m_ShowErrors = LogExtension.GUIToggleButton(m_ShowErrors,
                new GUIContent(m_ErrorCount <= 999 ? m_ErrorCount.ToString() : "999+",
                    m_ErrorCount == 0 ? m_ErrorIconSmallInactive : m_ErrorIconSmall), 35f);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawTopPanel()
        {
            m_MessagePanel = new Rect(0f, k_MenuBarHeight, position.width,
                (position.height * m_SizeRatio) - k_MenuBarHeight);
            GUILayout.BeginArea(m_MessagePanel);
            m_MessageScroll = GUILayout.BeginScrollView(m_MessageScroll);

            CreateLogs();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawResizer()
        {
            m_Resizer = new Rect(0, (position.height * m_SizeRatio) - k_ResizerHeight, position.width, 10f);
            var size = new Vector2(position.width, 2f);
            GUILayout.BeginArea(new Rect(m_Resizer.position + (Vector2.up * 5f), size), m_ResizeStyle);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(m_Resizer, MouseCursor.ResizeVertical);
        }

        private void DrawBottomPanel()
        {
            var height = position.height * (1f - m_SizeRatio);

            m_StacktracePanel = new Rect(0, (position.height * m_SizeRatio) + k_ResizerHeight, position.width, height);
            GUILayout.BeginArea(m_StacktracePanel);
            m_StacktraceScroll = GUILayout.BeginScrollView(m_StacktraceScroll);

            if (m_SelectedLog != null)
            {
                var stack = StacktraceHyperlink(m_SelectedLog.stackTrace);

                EditorGUILayout.SelectableLabel(stack, m_StacktraceStyle, GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true), GUILayout.MinHeight(35f));
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private bool DrawBox(string content, LogType logType, bool isLight, bool isSelected)
        {
            if (isSelected)
            {
                m_MessageStyle.normal.background = m_MessageSelected;
            }
            else
            {
                m_MessageStyle.normal.background = isLight ? m_MessageBackgroundLight : m_MessageBackgroundDark;
            }

            m_LOGIcon = logType switch
            {
                LogType.Error => m_ErrorIcon,
                LogType.Assert => m_WarningIcon,
                LogType.Warning => m_WarningIcon,
                LogType.Log => m_MessageIcon,
                LogType.Exception => m_ErrorIcon,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };

            return GUILayout.Button(new GUIContent(content, m_LOGIcon), m_MessageStyle,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(k_LogHeight));
        }

        private void CreateLogs()
        {
            for (var i = 0; i < m_Logs.Count; i++)
            {
                var tags = m_Logs[i].logTags;
                string message = m_Logs[i].message;

                if (m_DisableUnityLogs && tags[0].Equals("Unity")) continue;

                var searchValue = Search(tags, ref message);
                string content = CreateLogTags(tags, message);
                string fullContent = m_Logs[i].time + content;

                if (searchValue.Item1 < 0 || searchValue.Item2 < 0) continue;

                var logType = m_Logs[i].logType;
                switch (logType)
                {
                    case LogType.Log when !m_ShowMessages:
                    case LogType.Warning when !m_ShowWarnings:
                    case LogType.Error when !m_ShowErrors:
                    case LogType.Assert when !m_ShowWarnings:
                    case LogType.Exception when !m_ShowErrors:
                        continue;
                }

                if (DrawBox(fullContent, m_Logs[i].logType, i % 2 == 0, m_Logs[i].select))
                {
                    if (m_SelectedLog != null)
                    {
                        m_SelectedLog.select = false;
                    }

                    m_Logs[i].select = true;
                    m_SelectedLog = m_Logs[i];

                    if (m_SelectedLog.message.Equals(m_SelectedLog.stackTrace))
                    {
                        PingObjectInHierarchy(m_SelectedLog.message);
                    }
                }
            }
        }

        private string CreateLogTags(IReadOnlyList<string> tags, string message)
        {
            string content;

            if (tags.Count > 1)
            {
                s_stringBuilder.Clear();
                s_stringBuilder.Append($"\n[{tags[0]}]");

                for (var k = 1; k < tags.Count; k++)
                {
                    s_stringBuilder.Append($", [{tags[k]}]");
                }

                content = message + s_stringBuilder;
            }
            else if (!string.IsNullOrEmpty(tags[0]))
            {
                content = message + $"\n[{tags[0]}]";
            }
            else
            {
                content = message;
            }

            return content;
        }

        private (int, int) Search(IEnumerable<string> tags, ref string text)
        {
            m_SearchString ??= string.Empty;
            m_SearchTag ??= string.Empty;

            var searchValue = (0, 0);

            if (!string.IsNullOrEmpty(m_SearchTag))
            {
                searchValue.Item1 = GetHighestTagValue(tags);
            }

            var pattern = new StringBuilder();
            if (!string.IsNullOrEmpty(m_SearchString))
            {
                searchValue.Item2 = text.IndexOf(m_SearchString, StringComparison.OrdinalIgnoreCase);

                foreach (var character in m_SearchString)
                {
                    pattern.Append(character);
                }

                var str = Regex.Escape(pattern.ToString());
                const string replacement = "$&";
                text = Regex.Replace(text, str, LogExtension.Color(replacement, "red"), RegexOptions.IgnoreCase);
            }
            else
            {
                pattern.Clear();
            }

            return searchValue;
        }

        private void Resize(Event e)
        {
            if (!m_IsResizing) return;

            m_SizeRatio = e.mousePosition.y / position.height;

            // To ensure that the bottom panel can not be moved outside of the EditorWindow
            if (m_SizeRatio < 0.1f)
            {
                m_SizeRatio = 0.1f;
            }
            else if (m_SizeRatio > 0.9f)
            {
                m_SizeRatio = 0.9f;
            }
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && m_Resizer.Contains(e.mousePosition))
                    {
                        m_IsResizing = true;
                    }

                    break;

                case EventType.MouseUp:
                    m_IsResizing = false;
                    break;
            }

            ClearSearch(e);
            Resize(e);
        }

        private void ClearSearch(Event e)
        {
            if (e.keyCode is not KeyCode.Escape) return;
            
            if (GUI.GetNameOfFocusedControl() is "m_SearchString")
            {
                m_SearchString = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
            else if (GUI.GetNameOfFocusedControl() is "m_SearchTag")
            {
                m_SearchTag = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
        }

        private void UpdateLogCounts(LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    m_ErrorCount++;
                    break;
                case LogType.Assert:
                case LogType.Warning:
                    m_WarningCount++;
                    break;
                case LogType.Log:
                    m_MessageCount++;
                    break;
            }
        }

        private void SetLogRect()
        {
            if (m_SelectedLog == null) return;

            for (var i = 0; i < m_Logs.Count; i++)
            {
                if (m_Logs[i].Equals(m_SelectedLog))
                {
                    m_SelectedLog.rect = new Rect(0f, k_LogHeight * i - m_MessageScroll.y + 20f, position.width,
                        k_LogHeight);
                }
            }
        }

        private void ReceivedData(Log log, LogType logType)
        {
            m_Logs.Add(log);

            if (logType is LogType.Error or LogType.Exception or LogType.Assert && m_ErrorPause)
            {
                EditorApplication.isPaused = true;
            }

            UpdateLogCounts(logType);
            m_MessageScroll.y = m_Logs.Count * k_LogHeight;
        }

        private void ClearConsole()
        {
            m_MessageCount = m_WarningCount = m_ErrorCount = 0;
            m_SelectedLog = null;
            m_Logs.Clear();
        }

        private void OnRecompile()
        {
            if (m_ClearOnRecompile) ClearConsole();
        }

        private void OnBuild()
        {
            if (clearOnBuild) ClearConsole();
        }

        private void UnityLog(string condition, string stacktrace, LogType type)
        {
            var time = $"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}] ";
            var stack = stacktrace;
            var message = condition.Trim();

            if (string.IsNullOrEmpty(stacktrace))
            {
                stack = message;
            }

            var newLog = new Log(message, stack, new[] { "Unity", $"{type}" }, time, false, type);
            OnReceivedData?.Invoke(newLog, type);
        }

        public static void LogMessage(Log log)
        {
            OnReceivedData?.Invoke(log, log.logType);
        }

        private void OnPlayMode(PlayModeStateChange state)
        {
            if (m_ClearOnPlay && state is PlayModeStateChange.ExitingEditMode)
            {
                ClearConsole();
            }
        }

        private int GetHighestTagValue(IEnumerable<string> tags)
        {
            return tags.Select(tag => tag.IndexOf(m_SearchTag, StringComparison.OrdinalIgnoreCase)).Prepend(-1).Max();
        }

        private void PingObjectInHierarchy(string messageText)
        {
            var startIndex = messageText.IndexOf('\'');
            var endIndex = messageText.LastIndexOf('\'');

            if (startIndex < 0 || endIndex < 0) return;
            var objectName = messageText.Substring(startIndex, endIndex - startIndex).Replace("'", "");

            foreach (var gameObject in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (!gameObject.name.Equals(objectName)) continue;

                EditorGUIUtility.PingObject(gameObject);
            }
        }

        private void DoubleClickedLog()
        {
            const string asset = "<a href=assets ";
            var link = GetFirstHyperlink(m_SelectedLog.stackTrace);

            if (string.IsNullOrEmpty(link)) return;

            link = link.Remove(0, asset.Length);

            var startIndexForNumber = link.LastIndexOf("=", StringComparison.Ordinal);
            
            if (startIndexForNumber <= 0) return;
            
            var lineNumber = link.Substring(startIndexForNumber + 2, link.Length - startIndexForNumber - 3);

            var startIndex = link.IndexOf(@"""", StringComparison.Ordinal);
            link = link.Remove(startIndex, (link.Length - startIndex));

            var fileName = Application.dataPath + link;
            int.TryParse(lineNumber, out var value);

            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fileName, value);
        }

        private string GetFirstHyperlink(string stacktrace)
        {
            var hyperlink = new StringBuilder();
            var lines = stacktrace[..].Split(new[] { "\n" }, StringSplitOptions.None);
            const string textBeforeFilePath = " (at ";

            foreach (var line in lines)
            {
                var filePathIndex = line.IndexOf(textBeforeFilePath, StringComparison.Ordinal);
                
                if (filePathIndex > 0)
                {
                    filePathIndex += textBeforeFilePath.Length;

                    if (line[filePathIndex] != '<')
                    {
                        var filePathPart = line[filePathIndex..];
                        var lineIndex = filePathPart.LastIndexOf(":", StringComparison.Ordinal);

                        if (lineIndex > 0)
                        {
                            var endLineIndex = filePathPart.LastIndexOf(")", StringComparison.Ordinal);
                            
                            if (endLineIndex > 0)
                            {
                                var lineString =
                                    filePathPart.Substring(lineIndex + 1, (endLineIndex) - (lineIndex + 1));
                                var filePath = filePathPart[..lineIndex];

                                hyperlink.Append(
                                    $"{line[..filePathIndex]}" +
                                    $"<a href=\"{filePath}\" line=\"{lineString}\">{filePath}:{lineString}</a>)\n");
                                
                                continue;
                            }
                        }
                    }
                }

                hyperlink.Append(line + "\n");
            }

            if (hyperlink.Length > 0)
            {
                var str = hyperlink.ToString();
                // DoubleClick log failed, length cannot be less than 0 ?
                var length = str[..str.IndexOf(textBeforeFilePath, StringComparison.Ordinal)];
                hyperlink.Remove(0, length.Length + textBeforeFilePath.Length);
                str = hyperlink.ToString();

                var startIndex = str.IndexOf(">", StringComparison.Ordinal);

                if (startIndex > 0)
                {
                    var endIndex = str.Length - startIndex;
                    hyperlink.Remove(startIndex, endIndex);
                }
            }

            return hyperlink.ToString();
        }

        private string StacktraceHyperlink(string stacktraceText)
        {
            var hyperlink = new StringBuilder();
            var lines = stacktraceText[..].Split(new[] { "\n" }, StringSplitOptions.None);
            const string textBeforeFilePath = " (at ";

            foreach (var line in lines)
            {
                var filePathIndex = line.IndexOf(textBeforeFilePath, StringComparison.Ordinal);

                if (filePathIndex > 0)
                {
                    filePathIndex += textBeforeFilePath.Length;

                    if (line[filePathIndex] != '<')
                    {
                        var filePathPart = line[filePathIndex..];
                        var lineIndex = filePathPart.LastIndexOf(":", StringComparison.Ordinal);

                        if (lineIndex > 0)
                        {
                            var endLineIndex = filePathPart.LastIndexOf(")", StringComparison.Ordinal);

                            if (endLineIndex > 0)
                            {
                                var lineString =
                                    filePathPart.Substring(lineIndex + 1, (endLineIndex) - (lineIndex + 1));
                                var filePath = filePathPart[..lineIndex];

                                hyperlink.Append(
                                    $"{line[..filePathIndex]}" +
                                    $"<a href=\"{filePath}\" line=\"{lineString}\">{filePath}:{lineString}</a>)\n");

                                continue;
                            }
                        }
                    }
                }

                hyperlink.Append(line + "\n");
            }

            if (hyperlink.Length > 0)
            {
                hyperlink.Remove(hyperlink.Length - 1, 1);
            }
            
            return hyperlink.ToString();
        }
    }

    public class ClearOnBuildClass : IPreprocessBuildWithReport
    {
        public static Action clearConsoleOnBuild;
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            clearConsoleOnBuild?.Invoke();
        }
    }
}
#endif