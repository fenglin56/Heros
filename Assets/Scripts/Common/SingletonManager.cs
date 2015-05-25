using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Policy;

	 /// <summary>
    /// 单例管理器
    /// </summary>
	public class SingletonManager
	{
        private static SingletonManager m_instance;
        private Dictionary<ISingletonLifeCycle, bool> m_Singletons = new Dictionary<ISingletonLifeCycle, bool>();

        public static SingletonManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SingletonManager();
                }
                return m_instance;
            }
        }
        public void Add(ISingletonLifeCycle singletonLifeCycle)
        {
            Add(singletonLifeCycle, true);
        }
        public void Add(ISingletonLifeCycle singletonLifeCycle,bool setToNull)
        {
            if (singletonLifeCycle != null)
            {
                singletonLifeCycle.Instantiate();
				m_Singletons.Add(singletonLifeCycle, setToNull);
            }
        }
        public void Clear()
        {
		List<ISingletonLifeCycle> temp=new List<ISingletonLifeCycle>();
		this.m_Singletons.ApplyAllItem(P => 
            { 
                if (P.Key != null)
			{
					P.Key.LifeOver();
			}
                if(P.Value) 
			{
				temp.Add(P.Key);
			}
		});
		  temp.ApplyAllItem(P=>{
			this.m_Singletons.Remove(P);
		});

            System.GC.Collect();
	}
}
    /// <summary>
    /// 单例需要实现的接口
    /// </summary>
    public interface ISingletonLifeCycle
    {
        void Instantiate();
        void LifeOver();
    }
