/// <summary>
/// Copyright Aristocrat Technologies Australia Pty Ltd 1994-2015. 
/// All rights reserved.
/// 
/// DESCRIPTION:
/// Holds all of the targets used in the MessageObject object
/// </summary>
namespace Assets.Scripts.Utils
{
	public class Targets
	{
        /// <summary>
        /// Message should go to the Server class (possibly eventually to the 
        /// BaseGame class)
        /// </summary>
        public const string Server  = "Server";

        /// <summary>
        /// Message should go to the Client class (possibly eventually to the 
        /// UIBaseGame class)
        /// </summary>
        public const string Client = "Client";

        /// <summary>
        /// Message should go to the UITools class
        /// </summary>
        public const string Tools = "Tools";        
	}
}