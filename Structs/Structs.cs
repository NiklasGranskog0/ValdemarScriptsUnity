using System;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Framework.Structs
{
    #region Events

    public struct ExperienceEvent : IEvent
    {
        public int experience;
    }

    #endregion

    #region ObjectsForPooling

    [Serializable]
    public struct PoolStruct
    {
        public GameObject gameObject;
        public ObjectEnum objectEnum;
        public uint initialQuantity;
    }

    #endregion
}
