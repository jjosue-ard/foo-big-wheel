/// <summary>
/// Copyright Aristocrat Technologies Australia Pty Ltd 1994-2018. All rights reserved.
/// 
/// Commands used in messages (MessageObject). They tell the receiving class
/// what to do. See command descriptions below for more details
/// </summary>
namespace Assets.Scripts.Utils
{
	public class Commands
	{
		//Used in every message. It tells the receiver what kind of a 
		//message it is in order to parse it.
		public const string Command	= "command";

        public const string Target = "target";

        public const string DebugRequest = "testing123";

        public const string TargetReelSymbolReachedDestination = "TargetReelSymbolReachedDestination";
        public const string ReelReachedItsDestroyPoint = "ReelReachedItsDestroyPoint";
        //SocketClient
        public const string ConnectionSuccess = "ConnectionSuccess";

        //State commands
        //Sent to initialize the game engine
        public const string InitStateRequest = "initStateRequest";
        public const string ClientReadyState = "clientReadyState";
        public const string DealingState = "dealingState";            
        public const string UpdatePlayer = "updatePlayer";
        public const string TestRequest = "TestRequest";

        // A new player joined the game
        public const string PlayerJoinRequest = "playerJoinRequest";
        public const string PlayerJoinReply = "playerJoinReply";
        public const string PlayerLeaveRequest = "playerLeaveRequest";
        public const string PlayerLeaveReply = "playerLeaveReply";
        public const string PlayerReadyRequest = "playerReadyRequest";
        public const string NextSessionRequest = "NextSessionRequest";

        //SocketIO
        public const string SpinResult = "SpinResult";
        public const string ErrorMessageReceived = "ErrorMessageReceived";

    }

}