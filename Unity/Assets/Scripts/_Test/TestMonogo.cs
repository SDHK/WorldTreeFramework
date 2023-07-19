using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using WorldTree;




public class TestMonogo : MonoBehaviour
{
    public int a;
    public int b;
    public int c;
    
    
    
    // Start is called before the first frame update
    void Start()
    {

        
        
        // Debug.Log(Convert_ObjectToBytes(typeof(UnityWorldTree).AssemblyQualifiedName).Length);
        // foreach (var VARIABLE in Convert_ObjectToBytes(typeof(UnityWorldTree).AssemblyQualifiedName))
        // {
        //     Debug.Log(VARIABLE);
        // }


        var b = Encoding.Default.GetBytes(typeof(UnityWorldTree).AssemblyQualifiedName);
        Debug.Log(Type.GetType(Encoding.Default.GetString(b)));
        Debug.Log(b.Length);
    }
    private IMongoCollection<BsonDocument> DatabaseConn()
    {
        var client = new MongoClient("mongodb://localhost");
        var database = client.GetDatabase("SDHKTest"); //数据库名称
        var collection = database.GetCollection<BsonDocument>("A001"); //连接的表名

        return collection;
    }

    public void InsertData()
    {
        var document = new BsonDocument
        {
            { "aa", 11 },
            { "bb", 22 },
            {
                "cc", new BsonDocument
                {
                    { "x", 33 },
                    { "y", 44 }
                }
            }
        };
        var collection = DatabaseConn();
        collection.InsertOne(document);
    }
}