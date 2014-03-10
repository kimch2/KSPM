﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPM.Network.Common.Messages
{
    public class ManagedMessage : Message
    {
        /// <summary>
        /// A network entity which is owner of the message.
        /// </summary>
        protected NetworkEntity messageOwner;

        /// <summary>
        /// Creates an instance and set the NetworkEntity owner of this message.
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="messageOwner"></param>
        public ManagedMessage(CommandType commandType, NetworkEntity messageOwner) : base( commandType )
        {
            this.messageOwner = messageOwner;
        }

        /// <summary>
        /// Sets a new NetworkEntity owner for this message.
        /// </summary>
        /// <param name="messageOwner"></param>
        public void SetOwnerMessageNetworkEntity(ref NetworkEntity messageOwner)
        {
            this.messageOwner = messageOwner;
        }

        /// <summary>
        /// Return the current NetworkEntity owner of this message.
        /// </summary>
        public NetworkEntity OwnerNetworkEntity
        {
            get
            {
                return this.messageOwner;
            }
        }

        /// <summary>
        /// Sets messageOwner to null and he messageRawLength to 0.<b>If you want to clean everythin you have to set to null this reference.</b>
        /// </summary>
        public override void Release()
        {
            this.messageOwner = null;
            this.messageRawLength = 0;
        }

        public void CloneContent(ManagedMessage otherMessage)
        {
            System.Buffer.BlockCopy( otherMessage.OwnerNetworkEntity.ownerNetworkCollection.rawBuffer, 0,this.OwnerNetworkEntity.ownerNetworkCollection.rawBuffer, 0, (int)otherMessage.messageRawLength);
        }
    }
}
