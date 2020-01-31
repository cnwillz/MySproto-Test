using System;
using System.Threading;
using Net;
using Sproto;

namespace TestNet {
	public class TestSprotoTcpSocket {
		private SprotoTcpSocket.LogType _log;
		
		public void Run(SprotoTcpSocket.LogType log)
		{
//			string c2s = 
//@"
//.package {
//	type 0 : integer
//	session 1 : integer
//	ud 2 : integer
//}
//
//C2GS_Ping 1006 {
//	request {
//		str 0 : string
//	}
//}
//";
//			string s2c = 
//@"
//.package {
//	type 0 : integer
//	session 1 : integer
//	ud 2 : integer
//}
//
//GS2C_Pong 1108 {
//	request {
//		str 0 : string
//		time 1 : integer
//	}
//}
//";
			
			
			string c2s = 
				@"
.package {
	type 0 : integer
	session 1 : integer
}

handshake 1 {
	response {
		msg 0  : string
	}
}

get 2 {
	request {
		what 0 : string
	}
	response {
		result 0 : string
	}
}

set 3 {
	request {
		what 0 : string
		value 1 : string
	}
}

quit 4 {}

complex 5 {
	request {
		
	}
	response {
		
	}
}
";
			string s2c = 
				@"
.package {
	type 0 : integer
	session 1 : integer
}

heartbeat 1 {}
";
			_log = log;
			
			SprotoMgr C2S = SprotoParser.Parse(c2s);
			SprotoMgr S2C = SprotoParser.Parse(s2c);

//			foreach (string key in C2S.Types.Keys)
//			{
//				log(key);
//			}
			

			SprotoTcpSocket client  = new SprotoTcpSocket(S2C,C2S);
			client.OnConnect += this.OnConnect;
			client.OnClose += this.OnClose;
			client.Log = log;
			string host = "127.0.0.1";
//			string host = "111.230.108.129";
			int port = 8888;
			client.TcpSocket.Connect(host,port);

			client.Dispatcher.AddHandler("heartbeat", HandleHeartBeat);


			while (true) {
				client.TcpSocket.Dispatch();
				Thread.Sleep(100);
			}
		}

		private void SendHandShake(SprotoTcpSocket client)
		{
			SprotoObject request = client.Proto.C2S.NewSprotoObject("handshake.request");
			client.SendRequest("handshake",request, HandleHandshake);
		}
		
		private void SendSet(SprotoTcpSocket client)
		{
			SprotoObject request = client.Proto.C2S.NewSprotoObject("set.request");
			request["what"] = "hello";
			request["value"] = "world";
			client.SendRequest("set",request, HandleSetOK);
		}

		private void HandleHandshake(SprotoTcpSocket client,RpcMessage message) {
			_log(message.response["msg"]);
		}
		
		private void HandleHeartBeat(SprotoTcpSocket client,RpcMessage message) {
//			_log("session:" + message.session);
			_log("Receive heartbeat");
//			SprotoObject response = client.Proto.S2C.NewSprotoObject("heartbeat.response");
//			client.SendResponse("heartbeat",response, message.session);
		}

		private void HandleSetOK(SprotoTcpSocket client, RpcMessage message) {
			_log("set ok");
		}
		
		public void HandlerLoginPong(SprotoTcpSocket client,RpcMessage message) {
			// ping-pong loop
			string msg = String.Format("[{0}] op=OnMessage,proto={1},tag={2},ud={3},session={4},type={5},request={6},response={7}",
				client.TcpSocket.Name,message.proto,message.tag,message.ud,message.session,message.type,message.request,message.response);
			Console.WriteLine(msg);
			//Debug.Log(msg);
			SprotoObject request = client.Proto.C2S.NewSprotoObject("C2GS_Ping.request");
			request["str"] = "ping";
			client.SendRequest("C2GS_Ping",request);
		}

		private void OnConnect(SprotoTcpSocket client) {
			SendHandShake(client);
			SendSet(client);
		}

		private void OnClose(SprotoTcpSocket client) {
			//Console.WriteLine("OnClose");
		}

		private void LogSocket(string msg) {
			Console.WriteLine(msg);
			//Debug.Log(msg);
		}
	}
}
