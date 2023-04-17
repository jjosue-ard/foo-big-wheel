/// <summary>
/// Copyright Aristocrat Technologies Australia Pty Ltd 1994-2018. 
/// All rights reserved.
/// 
/// DESCRIPTION:
/// Holds common messages found in MessageObject
/// </summary>
using System;

namespace Assets.Scripts.Utils
{
	public class Messages
	{
        ///Descriptions describe the object their associated with

        /// <summary>
        /// The state of the BaseGame object (Init, Spinning)
        /// </summary>
        public const string BaseGameState  = "BaseGameState";

        /// <summary>
        /// The BaseGame object dispatched by the Server
        /// </summary>
        public const string BaseGame = "BaseGame";

        /// <summary>
        /// A string describing the gaff to use (e.g. Messages.GaffFreeSpins) 
        /// </summary>
        public const string Gaff = "Gaff";


        /// <summary>
        /// Gaff the feature assigned to the scatter
        /// </summary>
        public const string GaffFeature = "GaffFeature";

        /// <summary>
        /// Used to reset the credit meter
        /// </summary>
        public const string Credits = "Credits";

        public const string ReelStripInstance = "ReelStripInstance";

        public const string HandResult = "HandResult";
        public const string CardInfo = "CardInfo";
        public const string CardPos = "CardPos";
        public const string DealerInfo = "DealerInfo";

        public const string PlayerPositionOnTable = "PlayerPositionOnTable";
        public const string UpdatedPlayerObj = "UpdatedPlayerObj";
        public const string UpdatedWinObj = "UpdatedWinObj";
        public const string PlayerObject = "PlayerObject";
        public const string PlayerUID = "PlayerUID";
        public const string BetValue = "BetValue";
        public const string BetEvaluated = "BetEvaluated";

        public const string UIDraggableInstance = "UIDraggableInstance";
        public const string UITapChipInstance = "UITapChipInstance";
        public const string UITapZoneInstance = "UITapZoneInstance";
        public const string BetValueRemoved = "BetValueRemoved";
        public const string BetsObj = "BetsObj";

        public const string CardLabel = "CardLabel";

        public const string SpinResultsData = "SpinResultsData";
        public const string ErrorMessage = "ErrorMessage";
    }
}