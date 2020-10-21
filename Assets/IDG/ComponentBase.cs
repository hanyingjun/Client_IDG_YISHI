namespace IDG
{
    public class ComponentBase
    {
        public NetData netData { get; private set; }

        public ComponentBase()
        {
        }

        public virtual void Init()
        {
        }

        public void InitNetData(NetData data)
        {
            this.netData = data;
            Init();
        }

        public virtual void Update()
        {
        }
    }
}
