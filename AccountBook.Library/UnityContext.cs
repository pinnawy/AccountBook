using System;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace AccountBook.Library
{
    public class UnityContext
    {
        /// <summary>
        /// 注入Bll实现类
        /// </summary>
        /// <typeparam name="TBllModel">Bll接口类型</typeparam>
        /// <returns>Bll实现类</returns>
        public static TBllModel LoadBLLModel<TBllModel>()
        {
            return LoadModel<TBllModel>("BllContainer");
        }

        /// <summary>
        /// 注入DAL实现类
        /// </summary>
        /// <typeparam name="TDalModel">DAL接口类型</typeparam>
        /// <returns>DAL实现类</returns>
        public static TDalModel LoadDALModel<TDalModel>()
        {
            return LoadModel<TDalModel>("DalContainer");
        }

        /// <summary>
        /// 注入实现类
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="name">配置结点名称</param>
        /// <returns></returns>
        private static T LoadModel<T>(string name)
        {
            try
            {
                IUnityContainer container = new UnityContainer();
                var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                section.Containers[name].Configure(container);
                return container.Resolve<T>();
            }
            catch (Exception ex)
            {
                LogUtil.Log.Error("LoadModel Exception", ex);
                return default(T);
            }
        }
    }
}
