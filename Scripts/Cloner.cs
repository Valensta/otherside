using System.Collections.Generic;
using System;
using UnityEngine;


public interface IDeepCloneable
{
    object DeepClone();
}

public interface IDeepCloneable<T>: IDeepCloneable
{
    T DeepClone();
}

public class CloneUtil
{

    public static List<T> copyList<T>(List<T> list) where T : IDeepCloneable<T>
    {
        if (list == null) return null;
        List<T> new_list = new List<T>();

        for (int i = 0; i < list.Count; i++)
        {
            new_list.Add(list[i].DeepClone());
        }

        return new_list;
    }

}

 


public class EnumUtil { 

    public static List<TEnum> copyEnumList<TEnum>(List<TEnum> list) where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        if (list == null) return null;
        List<TEnum> new_list = new List<TEnum>();

        for (int i = 0; i < list.Count; i++)
        {
            new_list.Add(list[i]);
        }

        return new_list;
    }


    public static TEnum EnumFromString<TEnum>(string s, TEnum default_enum) where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        TEnum my_enum = default_enum;
        try
        {
            if (s != null)
                my_enum = (TEnum)System.Enum.Parse(typeof(TEnum), s);
        }
        catch
        {
            Debug.Log("Could not parse ArrowType " + s + "\n");
        }

        return my_enum;
    }

}
[System.Serializable]
public class MyArray<T> where T : MonoBehaviour
{
    public T[] array;
    public int total_count;
    public int max_count;
    

    public MyArray(int size){
       array = new T[size];
    }

    public MyArray()
    {
        array = new T[200];
        total_count = 200;
    }

    public bool containsGameObjectInArrayByID(T obj)
    {
        int check_ID = obj.GetInstanceID();
        
        for (int i = 0; i < max_count; i++)
        {
            if (array[i] == null) continue;
            GameObject checkme = array[i].gameObject;
            //if (!checkme.activeSelf) continue;
            if (checkme.GetInstanceID() == check_ID) return true;
        }
        return false;

    }

    public void removeByID(T obj)
    {
   //     Debug.Log("Want to remove " + obj.gameObject.name + " " + obj.gameObject.GetInstanceID() + " have " + total_count + " total\n");
        int remove_me = obj.gameObject.GetInstanceID();
        for (int i = 0; i < total_count; i++)
        {
            if (array[i] == null) continue;
        //    Debug.Log("Checking " + array[i].GetInstanceID() + "\n");
            if (array[i].gameObject.GetInstanceID() == remove_me)
            {
             //   Debug.Log("Removed " + obj.gameObject.name + "\n");
                array[i] = null;
            }            
        }

    }


    public void addByID(T obj)
    {
        if (containsGameObjectInArrayByID(obj)) return;
        bool good = false;

        for (int i = 0; i < total_count; i++)
        {
            if (array[i] == null)
            {
                array[i] = obj;
         //       Debug.Log("Added Object by ID to " + i + "\n");
                if (max_count < i + 1) max_count = i + 1;
                /*
                for (int x = 0; x < max_count; x++)
                {
                    if (array[x] != null)
                        Debug.Log(x + " " + array[x].gameObject.name + " " + array[x].gameObject.GetInstanceID() + " is active " + array[x].gameObject.activeSelf + "\n");
                }
                */
                
                return;
            }
           
        }
/*
        for (int x = 0; x < max_count; x++)
        {
            if (array[x] != null)
                Debug.Log(x + " " + array[x].gameObject.name + " is active " + array[x].gameObject.activeSelf + "\n");
        }
        */
        if (good) return;
        int length = array.Length;
        
        Array.Resize(ref array, array.Length + Mathf.Max(4, length / 4));
        total_count = array.Length;
        Debug.Log("RESIZING AN ARRAY WHY from " + length + " to " + total_count + "\n");
        array[length] = obj;
 //       Debug.Log("Added Object by ID to " + length + "\n");
        max_count = length + 1;
        

    }
}