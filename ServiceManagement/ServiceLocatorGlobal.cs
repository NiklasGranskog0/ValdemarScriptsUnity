namespace Assets.Scripts.Framework.ServiceManagement
{
    public class ServiceLocatorGlobal : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal();
        }
    }
}
