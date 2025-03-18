using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandle
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandle(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public void Save(GameData _data)
    {
        //拼接文件夹路径和文件名
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //寻找该文件，如果没有就创建
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //将GameData转换为Json字符串;
            string dataToStore = JsonUtility.ToJson(_data, true);
            //创建一个文件流，将Json数据写入文件
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("试图保存到文件" + fullPath + "发生错误" + "\n" + e);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadData = null;

        //如果存在该文件
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using(FileStream stream = new FileStream(fullPath,FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();   
                    }
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("试图从" + fullPath + "加载数据失败" + "\n" + e);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
