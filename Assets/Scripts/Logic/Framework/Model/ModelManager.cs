using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ModelManager
{
    private static Dictionary<string, Model> m_modelMap = new Dictionary<string, Model>();

    //添加一个模型
    public static bool AddModel(Model m)
    {
        if (m_modelMap.ContainsKey(m.GetModelName()))
        {
            m_modelMap.Add(m.GetModelName(), m);
            return true;
        }

        return false;
    }

    //获取一个模型
    public static Model GetModel(string modelName)
    {
        if (m_modelMap.ContainsKey(modelName))
        {
            return m_modelMap[modelName];
        }

        return null;
    }

    //删除一个模型
    public static void DelModel(string modelName)
    {
        m_modelMap.Remove(modelName);
    }
}

