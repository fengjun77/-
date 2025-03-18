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
        //ƴ���ļ���·�����ļ���
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //Ѱ�Ҹ��ļ������û�оʹ���
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //��GameDataת��ΪJson�ַ���;
            string dataToStore = JsonUtility.ToJson(_data, true);
            //����һ���ļ�������Json����д���ļ�
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
            Debug.LogError("��ͼ���浽�ļ�" + fullPath + "��������" + "\n" + e);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath,dataFileName);
        GameData loadData = null;

        //������ڸ��ļ�
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
                Debug.LogError("��ͼ��" + fullPath + "��������ʧ��" + "\n" + e);
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
