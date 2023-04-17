/// <summary>
/// Copyright Aristocrat Technologies Australia Pty Ltd 1994-2018. 
/// All rights reserved.
/// 
/// DESCRIPTION:
/// An object dispatched from child objects to parent objects containing
/// relevant information
/// 
/// Child object creates a message object:
///     MessageObject<string, object> messageObject = new MessageObject<string, object>();
///     messageObject.Add(Commands.Command, Commands.BeginStateRequest);
///     messageObject.Add(Messages.BaseGameState, States.Spinning);
///     
/// Child object dispatches it to parent object:
///     EventManagerEventArgs args = new EventManagerEventArgs();
///     args.eventObject = messageObject;
///     EventManager.Instance.DispatchEvent(this, CustomEvent.Event, args);
///     
/// Parent object handles event message and parses message object:
///     private void messageEventHandler(object sender, EventManagerEventArgs e)
///     {
///        MessageObject<string, object> messageObject = (MessageObject<string, object>)e.eventObject;
///        string command = (string)messageObject[Commands.Command];
///
///         switch (command)
///         {
///            case Commands.InitStateRequest:
///               LoadClient(messageObject);
///               break;
///            case Commands.BeginStateRequest:
///               LoadClientGameState(messageObject);
///               break;
///            case Commands.CreditsResetRequest:
///               ClientCreditReset(messageObject);
///               break;
///         }
///     }
/// </summary>
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
	public class MessageObject<TKey,TValue> : Dictionary<string, TValue>
	{
		public MessageObject():base()
		{
            
		}

        public MessageObject(int capacity) : base(capacity)
        {

        }
	}
}