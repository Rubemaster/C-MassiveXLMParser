using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Node : IEquatable<Node>
    {
        public string pathName { get; set; }
        public int pathId { get; set; }
        public Dictionary<string, int> pathNodeCount {get; set;}
        public int parent { get; set; }
        public List<int> children { get; set; }
        public bool open {get; set;}
        public bool close {get; set;}
        public Dictionary<string, string> valueDictionary {get; set;}

        public override string ToString()
        {
            string values="";
            foreach (KeyValuePair<string, string> kvp in valueDictionary)
                values+="\n\t"+kvp.Key+"="+ kvp.Value;
            return 
                "Dir: " + parent +
                " ID: " + pathId +
                " Name: \"" + pathName+"\"" +
                " [ " + ( open ? close ? "Single" : "Open" : "Close" )+" ]"+values+"\n";
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Node objAsPath = obj as Node;
            if (objAsPath == null) return false;
            else return Equals(objAsPath);
        }
        public override int GetHashCode()
        {
            return pathId;
        }
        public bool Equals(Node other)
        {
            if (other == null) return false;
            return (this.pathId.Equals(other.pathId));
        }
        public Node(string name, int id, int parentId, bool openIn, bool closeIn, Dictionary<string, string> valueDictionaryIn=null){
            pathName=name;
            pathId=id;
            parent=parentId;
            pathNodeCount=new Dictionary<string, int>();
            children=new List<int>();
            open=openIn;
            close=closeIn;
            if(valueDictionaryIn==null){
                valueDictionary=new Dictionary<string,string>();
            }else{
                valueDictionary=valueDictionaryIn;
            }
        }
    }
class MainClass {
  private static string streamUntil(char stopAt, System.IO.StreamReader stream){
    bool stop=false;
    char thisIn=' ';
    string result="";
    do
    {
        thisIn = (char)stream.Read();
        result+=thisIn;
        if(thisIn==stopAt){stop=true;}
    } while (!stream.EndOfStream&&!stop); 
    return result;
  }
  private static void readOSM( System.IO.StreamReader stream){

    List<Node> allPaths= new List<Node>();
    int idCount=0;
    int thisPath=-1;
    do
    {
        streamUntil('<',stream);
        string element=new string(new char[] {(char)stream.Read()});
        Console.Clear();
        if(element=="/"){
            string elementName=streamUntil('>',stream).TrimEnd('>');
            thisPath=allPaths[thisPath].parent;
            allPaths.Add(new Node(elementName,idCount,thisPath,false,true));
            idCount++;
            Console.WriteLine(allPaths[allPaths.Count-1].ToString());
            
        }else{
            string elementName=element+streamUntil(' ',stream).TrimEnd(' ');
            string elementRest=streamUntil('>',stream).TrimEnd('>');
            char endsWith=elementRest[elementRest.Length-1];
            bool selfClosing=endsWith=='/'?true:false;

            elementRest=selfClosing?elementRest.TrimEnd('/'):elementRest;

            IEnumerable<string[]> items = elementRest.Split(new[] { ' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { '=' }));

            Dictionary<string, string> valueDictionary = new Dictionary<string, string>();
            foreach (string[] item in items)
                if(item.Length==2)
                    valueDictionary.Add(item[0], item[1]);
            
            allPaths.Add(new Node(elementName,idCount,thisPath,true,selfClosing?true:false,valueDictionary));
            if(thisPath!=-1){
                allPaths[thisPath].children.Add(allPaths[allPaths.Count-1].pathId);
            }
            Console.WriteLine( 
                allPaths[allPaths.Count-1].ToString()
            );
            

            
            if(!selfClosing)thisPath=idCount;
            idCount++;
        }
        Console.ReadKey();
    } while (!stream.EndOfStream); 
  }
  public static void Main (string[] args) {
    string path="Map data/Nodes";
    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }
    
    System.IO.StreamReader file =
        new System.IO.StreamReader(@"map.osm");
    readOSM(file);
    file.Close();  
  }
}