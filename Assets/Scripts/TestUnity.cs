using System.Collections;
using System.Threading;
using TestNet;
using UnityEngine;

public class TestUnity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
//        Debug.Log("test");
        
        var thread = new Thread(Run);
        thread.Start();
//        StartCoroutine("Run");
        
//        TestNet.Program.Main(null);
    }

    void Run()
    {
        TestSprotoTcpSocket tester2 = new TestSprotoTcpSocket();
        tester2.Run(Debug.Log);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
