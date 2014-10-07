﻿
using KSPM.Network.Server;
using KSPM.Network.Common.Packet;
using KSPM.Network.Client;
using KSPM.Game;
using KSPM.Globals;

namespace KSPM.Network.Common.Messages
{
    /// <summary>
    /// Abstrac class to represent a single message, holds the basic information required to work.
    /// Revision 2.0
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// An enum representing what kind of commands could be handled by the server and the client.
        /// Its header is composed in this way [ [HeaderIdentifier {byte:4}][MessageLength {byte:4}][ MessageId {byte:4} ][ Command {byte:1} ] ] so at least you have 13 unmovable positioned bytes.
        /// Its finel header is composed in this way [ [EndHeader {byte:4}] ]
        /// Do not modify these values unless you really are pretty sure about what you are doing.
        /// Also each command has a priority level, the lowest the level number the highest priority.
        /// 0 - Critical priority -> Connection commands.
        /// 1 - High priority -> User commands. These commands are passed to the upside level.
        /// 2 - At this moment this level has no commands.
        /// 3 - Chat commands, the lowest priority on the system.
        /// </summary>
        public enum CommandType : byte
        {
            /// <summary>
            /// These commands belongs to the 0 level of priority.
            /// </summary>
            #region LEVEL_0 Ids range[0:63]

            Null = 0,

            /// <summary>
            /// Void message, so if you are receiving this kind of messages means that something is wrong.
            /// </summary>
            Unknown,
            #region ServerCommands

            /// <summary>
            /// Its purpose is to tell the server to shutdown itself.
            /// </summary>
            StopServer,

            /// <summary>
            /// Restarts the server.<b>It is not implemeneted.</b>
            /// </summary>
            RestartServer,
            #endregion

            #region AuthenticationCommands
            /// <summary>
            /// Handshake command used to begin a connection between the server and the client.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            Handshake,

            /// <summary>
            /// NewClient command used by the client to try to stablish a connection with the server.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            NewClient,

            /// <summary>
            /// Means that the server just droped the connection.
            /// </summary>
            RefuseConnection,

            /// <summary>
            /// Message sent when the server is full and a new client is attempting to connect to the game.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            ServerFull,

            /// <summary>
            /// Command used by the client to send its authentication information.
            /// [Header {byte:4}][ Command {byte:1} ][ UsernameLenght {byte:1}] [ Username {byte:1-} ][HashLength{2}][ HashedUsernameAndPassword {byte:1-} ][ EndOfMessage {byte:4} ]
            /// </summary>
            Authentication,

            /// <summary>
            /// Command to tells that something went wrong while the authentication process.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            AuthenticationFail,

            /// <summary>
            /// Command to tells that the access is granted.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            AuthenticationSuccess,
            #endregion

            #region UserInteractionCommands

            #region UDPSettingUp
            /// <summary>
            /// Command sent by the server to tell the remote client wich port has been assigned to it also sends the pairing code. Either it works to test the connection.
            /// [Header {byte:4}][ Command {byte:1} ][ PortNumber{byte:4}][ PairingCode {byte:4} ][ EndOfMessage {byte:4} ]
            /// </summary>
            UDPSettingUp,

            /// <summary>
            /// Command send by the remote client to test the UDP connection, and the client establishes the message structure. <b>*It is sent through the UDP socket.*</b>
            /// [Header {byte:4}][ Command {byte:1} ][ PairingNumber{byte:4} ][ EndOfMessage {byte:4} ]
            /// </summary>
            UDPPairing,

            /// <summary>
            /// Command sent by the server to tell the remote client that everything is ok.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            UDPPairingOk,

            /// <summary>
            /// Command sent by the server to tell the remote client that its message has been received but its pairing code was wrong, anyway the connection works.
            /// [Header {byte:4}][ Command {byte:1} ]
            UDPPairingFail,

            /// <summary>
            /// 
            /// </summary>
            UDPBroadcast,

            #endregion

            /// <summary>
            /// Tells the remote client how many chat groups are registered inside the server.
            /// [Header {byte:4}][ Command {byte:1} ][ ChatGroupsCount { byte:2 } ] ( [ChatGroupId {byte:2} ] [ ChatGroupNameLength{byte:}][ ChatGroupName{byte:1-}] ) ... [ EndOfMessage {byte:4} ]
            /// </summary>
            ChatSettingUp,

            /// <summary>
            /// Resets the TCP timer and avoids the TimedOut socket error.<b>TCP timeout exception ocurs after 8 hours of inactivity.</b>
            /// [MessageHeader{ byte:4}][Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            KeepAlive,

            /// <summary>
            /// Disconnect command to a nicely way to say goodbye.
            /// [Header {byte:4}][ Command {byte:1} ][ EndOfMessage {byte:4} ]
            /// </summary>
            Disconnect,
            #endregion

            #endregion

            /// <summary>
            /// These commands belongs to the 1st level of priority.
            /// </summary>
            #region LEVEL_1 Ids range[64:127]

            /// <summary>
            /// Command used to mark the message and bypass it to the app. This is the empty message.
            /// [MessageHeader {byte:13}][UserCommand {byte:1}][Targets {byte:4}]......Whatever you need........[ EndOfMessage {byte:4} ]
            /// </summary>
            User = 64,

            #endregion

            /// <summary>
            /// These commands belongs to the 2nd level of priority.
            /// </summary>
            #region LEVEL_2 Ids range[128:191]
            #endregion

            /// <summary>
            /// These commands belongs to the 3rd level of priority.
            /// </summary>
            #region LEVEL_3 Ids range[192:255]

            /// <summary>
            /// Chat command.
            /// [MessageHeader {byte:13}][ From ( [ HashLength{ byte:2 } ][HashedId {byte:1-} ] ) ] [ GroupId{byte:2}] [MessageLength{byte:2}][ MessageBody{byte1:-}] [ EndOfMessage {byte:4} ]
            /// </summary>
            Chat = 192,

            /// <summary>
            /// UDP Chat command. A chat message sent through the UDP connection.
            /// [MessageHeader {byte:13}][ From ( [ HashLength{ byte:2 } ][HashedId {byte:1-} ] ) ] [ GroupId{byte:2}] [MessageLength{byte:2}][ MessageBody{byte1:-}] [ EndOfMessage {byte:4} ]
            /// </summary>
            UDPChat,

            #endregion
        }

        /// <summary>
        /// 4 bytes to mark the end of the message, is kind of the differential manchester encoding plus 1.
        /// </summary>
        public static readonly byte[] EndOfMessageCommand = new byte[] { 127, 255, 127, 0 };

        /// <summary>
        /// 4 bytes to mark the beggining of the message
        /// </summary>
        public static readonly byte[] HeaderOfMessageCommand = new byte[] { 127, 0, 255, 127 };

        /// <summary>
        /// Message counter to set to an unique id to each message.<b>Is defined as Int because System.Threading.Interlocked.Increment method only handles autommatically the overflow on Int32.</b>
        /// </summary>
        public static int MessageCounter = 0;
        
        /// <summary>
        /// Command type
        /// </summary>
        protected CommandType command;

        /// <summary>
        /// Message id to be used whatever you want, like server message sync or stuff like that.
        /// </summary>
        public uint MessageId;

        /// <summary>
        /// Holds the priority level of the message.
        /// </summary>
        public KSPMSystem.PriorityLevel Priority;

        /// <summary>
        /// Byte value used to sent user defined commands.
        /// </summary>
        public byte UserDefinedCommand;

        /// <summary>
        /// How many bytes of the buffer are usable, only used when the messages is being sent. This can hold up to 65535.
        /// </summary>
        protected uint messageRawLength;

        /// <summary>
        /// Tells if this messages is going to be broadcasted, so a different release will be performed.
        /// </summary>
        protected bool broadcasted;

        /// <summary>
        /// Will hold the body of the message, to avoid overwriting messages.
        /// </summary>
        public byte[] bodyMessage;

        /// <summary>
        /// Creates a new object and sets each property to a default and unusable values.<b>The bodyMessage is set to Null, BE CAREFUL WITH THAT.</b>
        /// </summary>
        /// <param name="kindOfMessage">Command kind</param>
        /// <param name="messageOwner">Network entity who is owner of this message.</param>
        public Message(CommandType kindOfMessage)
        {
            this.command = kindOfMessage;
            this.messageRawLength = 0;
            this.bodyMessage = null;
            this.broadcasted = false;
            this.MessageId = 0;
            this.Priority = KSPMSystem.PriorityLevel.Disposable;
        }

        /// <summary>
        /// Gets the command type of this message.
        /// </summary>
        public CommandType Command
        {
            get
            {
                return this.command;
            }
        }

        /// <summary>
        /// Gets or sets the amount of usable bytes inside the buffer and that amount of bytes are going to be sent.
        /// Use this property instead of the ServerSettings.ServerBufferSize property.
        /// </summary>
        public uint MessageBytesSize
        {
            get
            {
                return this.messageRawLength;
            }
            set
            {
                this.messageRawLength = value;
            }
        }

        /// <summary>
        /// Sets/Gets if the message is going to be broadcasted or not.<b>Be carefull setting this flag.</b>
        /// </summary>
        public bool IsBroadcast
        {
            get
            {
                return this.broadcasted;
            }
            set
            {
                this.broadcasted = value;
            }
        }

        /// <summary>
        /// Sets the bodymessage from another byte array cloning the array itself into its own buffer.
        /// </summary>
        /// <param name="rawBytes">Reference to the original buffer which is going to be cloned.</param>
        /// <param name="blockSize">Amount of bytes to be cloned.</param>
        /// <returns>The message's length.</returns>
        public uint SetBodyMessage(byte[] rawBytes, uint blockSize)
        {
            this.bodyMessage = new byte[blockSize];
            System.Buffer.BlockCopy(rawBytes, 0, this.bodyMessage, 0, (int)blockSize);
            this.messageRawLength = blockSize;
            return this.messageRawLength;
        }

        /// <summary>
        /// Sets the bodymessage from another byte array and the given offset, cloning the array into its own buffer.<b>Creates a new byte array.</b>
        /// </summary>
        /// <param name="rawBytes">Source byte array.</param>
        /// <param name="rawBytesOffset">Offset, index to indicate where to start to copying.</param>
        /// <param name="blockSize">Amount of bytes to be copied.</param>
        /// <returns></returns>
        public uint SetBodyMessage(byte[] rawBytes, uint rawBytesOffset, uint blockSize)
        {
            this.bodyMessage = new byte[blockSize];
            System.Buffer.BlockCopy(rawBytes, (int)rawBytesOffset, this.bodyMessage, 0, (int)blockSize);
            this.messageRawLength = blockSize;
            return this.messageRawLength;
        }

        /// <summary>
        /// Sets the body message with the given byte array reference.<b>Only copies the reference BE careful with that. DOES NOT create a new array.</b>
        /// </summary>
        /// <param name="rawBytes">Reference to the byte array.</param>
        /// <param name="blockSize">Amount of bytes to be used.</param>
        /// <returns></returns>
        public uint SetBodyMessageNoClone(byte[] rawBytes, uint blockSize)
        {
            this.bodyMessage = rawBytes;
            this.messageRawLength = blockSize;
            return this.messageRawLength;
        }

        /// <summary>
        /// Gets a basic information of the message.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} Id: [{1}], [{2}] Command, [{3}] bytes length, PriorityLevel: [{4}]", this.GetType().ToString(), this.MessageId.ToString(), this.command.ToString(), this.messageRawLength, this.Priority);
        }

        /// <summary>
        /// Abstract method that MUST be used to release all resources on this reference.
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// Abstract method that MUST be used to dipose whatever resource you need.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Abstract method that MUST be used to create Empty references.
        /// </summary>
        /// <returns></returns>
        public abstract Message Empty();

        /// <summary>
        /// Gets the command priority according to its command id.
        /// </summary>
        /// <param name="commandId">The command id as byte type.</param>
        /// <returns>Command group, goes from [0-3], Default 0.</returns>
        public static byte CommandPriority(byte commandId)
        {
            byte checker;
            byte groupFlag = 192;
            byte group = byte.MaxValue;
            checker = (byte)(groupFlag & commandId);
            if (checker == groupFlag)
            {
                group = 3;
            }
            else
            {
                group = 2;
                groupFlag = 128;
                while (groupFlag > 32)
                {
                    checker = (byte)(groupFlag & commandId);
                    if (checker == groupFlag)
                    {
                        break;
                    }
                    groupFlag = (byte)(groupFlag >> 1);
                    group--;
                }
            }
            return group;
        }

        #region AuthenticationCode

        /// <summary>
        /// Writes a handshake message in a raw format into the sender's buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType HandshakeAccetpMessage( NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte [] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.Handshake;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes( bytesToSend );
            System.Buffer.BlockCopy( messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length );

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Writes a NewUser message in a raw format into the sender's buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType NewUserMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.NewClient;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }


        /// <summary>
        /// Writes a handshake message in a raw format into the sender's buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType ServerFullMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.ServerFull;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Creates an authentication message. **In this moment it is not complete and may change in future updates.**
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="targetMessage"></param>
        /// <returns></returns>
        public static Error.ErrorType AuthenticationMessage(NetworkEntity sender, User userInfo, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            short hashSize;
            targetMessage = null;
            byte[] messageHeaderContent = null;
            byte[] userBuffer = null;
            string stringBuffer;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            stringBuffer = userInfo.Username;
            User.EncodeUsernameToBytes(ref stringBuffer, out userBuffer);

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.Authentication;
            bytesToSend += 1;

            ///Writing the username's byte length.
            rawBuffer[bytesToSend] = (byte)userBuffer.Length;
            bytesToSend += 1;

            ///Writing the username's bytes
            System.Buffer.BlockCopy(userBuffer, 0, rawBuffer, bytesToSend, userBuffer.Length);
            bytesToSend += userBuffer.Length;

            ///Writing the hash's length
            hashSize = (short)userInfo.Hash.Length;
            userBuffer = null;
            userBuffer = System.BitConverter.GetBytes(hashSize);
            System.Buffer.BlockCopy(userBuffer, 0, rawBuffer, bytesToSend, userBuffer.Length);
            bytesToSend += userBuffer.Length;

            ///Writing the user's hash code.
            System.Buffer.BlockCopy(userInfo.Hash, 0, rawBuffer, bytesToSend, hashSize);
            bytesToSend += hashSize;

            ///Writing the EndOfMessage command.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessage( rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Writes a AuthenticationFail message in a raw format into the sender's buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType AuthenticationFailMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.AuthenticationFail;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Writes a AuthenticationSuccess message in a raw format into the sender's buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType AuthenticationSuccessMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.AuthenticationSuccess;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        #endregion

        #region UserInteractionCode

        /// <summary>
        /// Writes an empty message in a raw format into the sender's buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// It is used because the TCP socket closes itself after 8 hours of inactivity.
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType KeepAlive(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.KeepAlive;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Writes a disconnect message into de buffer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="targetMessage"></param>
        /// <returns></returns>
        public static Error.ErrorType DisconnectMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.Disconnect;
            bytesToSend += 1;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Creating the Message
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessageNoClone(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Creates a message holding the required information by the remote client to get up its own chat system.
        /// </summary>
        /// <param name="sender">NetworkEntity who is the owner of this message.</param>
        /// <param name="availableGroups">List of available chat groups in the moment of the client joining.</param>
        /// <param name="targetMessage">Out reference to the message who will hold the infomation.</param>
        /// <returns></returns>
        public static Error.ErrorType SettingUpChatSystem(NetworkEntity sender, System.Collections.Generic.List<Chat.Group.ChatGroup> availableGroups, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            short shortBuffer;
            targetMessage = null;
            byte[] messageHeaderContent = null;
            byte[] bytesBuffer = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the command.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.ChatSettingUp;
            bytesToSend += 1;

            ///Writing how many chat groups are available.
            shortBuffer = (short)availableGroups.Count;
            bytesBuffer = System.BitConverter.GetBytes(shortBuffer);
            System.Buffer.BlockCopy(bytesBuffer, 0, rawBuffer, bytesToSend, bytesBuffer.Length);
            bytesToSend += bytesBuffer.Length;

            for (int i = 0; i < availableGroups.Count; i++)
            {
                ///Writing the group Id
                bytesBuffer = System.BitConverter.GetBytes(availableGroups[i].Id);
                System.Buffer.BlockCopy(bytesBuffer, 0, rawBuffer, bytesToSend, bytesBuffer.Length);
                bytesToSend += bytesBuffer.Length;

                ///Writing the chat name length
                KSPM.Globals.KSPMGlobals.Globals.StringEncoder.GetBytes(availableGroups[i].Name, out bytesBuffer);
                rawBuffer[bytesToSend] = (byte)bytesBuffer.Length;
                bytesToSend++;

                ///Writing the chat's name.
                System.Buffer.BlockCopy(bytesBuffer, 0, rawBuffer, bytesToSend, bytesBuffer.Length);
                bytesToSend += bytesBuffer.Length;
            }

            ///Writing the EndOfMessage command.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessage( rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        #endregion

        #region UDPCommands

        /// <summary>
        /// Creates a message with the required information by the remote client to stablish a connection using the UDP channel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="targetMessage"></param>
        /// <returns></returns>
        public static Error.ErrorType UDPSettingUpMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            int intBuffer;
            ServerSideClient ssClientReference = (ServerSideClient)sender;
            targetMessage = null;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPSettingUp;
            bytesToSend += 1;

            ///Writing the port number.
            intBuffer = ((System.Net.IPEndPoint)ssClientReference.udpCollection.socketReference.LocalEndPoint).Port;
            byteBuffer = System.BitConverter.GetBytes(intBuffer);
            System.Buffer.BlockCopy(byteBuffer, 0, rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writintg the paring code.
            byteBuffer = System.BitConverter.GetBytes(ssClientReference.CreatePairingCode());
            System.Buffer.BlockCopy(byteBuffer, 0, rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writing the users id generated by the system.
            byteBuffer = System.BitConverter.GetBytes(ssClientReference.gameUser.Id);
            System.Buffer.BlockCopy(byteBuffer, 0, rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;
            
            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessage( rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /*
        /// <summary>
        /// Writes an UDPParingMessage message in a raw format into the sender's udp buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType UDPPairingMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            GameClient gameClientReference = (GameClient)sender;
            targetMessage = null;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            gameClientReference.udpNetworkCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairing;
            bytesToSend += 1;

            ///Writing the pairing number.
            byteBuffer = System.BitConverter.GetBytes(gameClientReference.PairingCode);
            System.Buffer.BlockCopy(byteBuffer, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, gameClientReference.udpNetworkCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new RawMessage((CommandType)gameClientReference.udpNetworkCollection.rawBuffer[Message.HeaderOfMessageCommand.Length + 8], gameClientReference.udpNetworkCollection.rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }
        */

        /// <summary>
        /// Writes an UDPParingMessage message in a raw format into the sender's udp buffer then loads a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType LoadUDPPairingMessage(NetworkEntity sender, ref Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            GameClient gameClientReference = (GameClient)sender;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            gameClientReference.udpNetworkCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairing;
            bytesToSend += 1;

            ///Writing the pairing number.
            byteBuffer = System.BitConverter.GetBytes(gameClientReference.PairingCode);
            System.Buffer.BlockCopy(byteBuffer, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, gameClientReference.udpNetworkCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            System.Buffer.BlockCopy(gameClientReference.udpNetworkCollection.rawBuffer, 0, targetMessage.bodyMessage, 0, bytesToSend);
            targetMessage.messageRawLength = (uint)bytesToSend;
            targetMessage.command = (Message.CommandType)gameClientReference.udpNetworkCollection.rawBuffer[PacketHandler.PrefixSize + 4];

            //targetMessage = new RawMessage((CommandType)gameClientReference.udpNetworkCollection.rawBuffer[Message.HeaderOfMessageCommand.Length + 4], gameClientReference.udpNetworkCollection.rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Writes an UDPParingMessage message in a raw format into the sender's udp buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType LoadUDPInfoAndPairingMessage(NetworkEntity sender, ref Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            GameClient gameClientReference = (GameClient)sender;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            gameClientReference.udpNetworkCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairing;
            bytesToSend += 1;

            ///Writing the UDP IpEndPoint.Address used by the caller of this message.
            byteBuffer = ((System.Net.IPEndPoint)(gameClientReference.udpNetworkCollection.socketReference.LocalEndPoint)).Address.GetAddressBytes();
            ///Writing the lenghr of the address itself, giving support to IPv6 addresses.
            gameClientReference.udpNetworkCollection.rawBuffer[bytesToSend] = (byte)byteBuffer.Length;
            bytesToSend += 1;
            System.Buffer.BlockCopy(byteBuffer, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writing the UDP IpEndPoint.Port used by the caller of this message.
            byteBuffer = System.BitConverter.GetBytes(((System.Net.IPEndPoint)gameClientReference.udpNetworkCollection.socketReference.LocalEndPoint).Port);
            System.Buffer.BlockCopy(byteBuffer, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writing the pairing number.
            byteBuffer = System.BitConverter.GetBytes(gameClientReference.PairingCode);
            System.Buffer.BlockCopy(byteBuffer, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, byteBuffer.Length);
            bytesToSend += byteBuffer.Length;

            ///Writing the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, gameClientReference.udpNetworkCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            System.Buffer.BlockCopy(gameClientReference.udpNetworkCollection.rawBuffer, 0, targetMessage.bodyMessage, 0, bytesToSend);
            targetMessage.messageRawLength = (uint)bytesToSend;
            targetMessage.command = (Message.CommandType)gameClientReference.udpNetworkCollection.rawBuffer[PacketHandler.PrefixSize + 4];

            //targetMessage = new RawMessage((CommandType)gameClientReference.udpNetworkCollection.rawBuffer[Message.HeaderOfMessageCommand.Length + 4], gameClientReference.udpNetworkCollection.rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /*
        /// <summary>
        /// Writes an UDPParingOkMessage message in a raw format into the sender's udp buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType UDPPairingOkMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            ServerSideClient ssClientReference = (ServerSideClient)sender;
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            ssClientReference.udpCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairingOk;
            bytesToSend += 1;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, ssClientReference.udpCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new RawMessage((CommandType)ssClientReference.udpCollection.rawBuffer[Message.HeaderOfMessageCommand.Length + 8], ssClientReference.udpCollection.rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }
        */

        /// Writes an UDPParingOkMessage message in a raw format into the sender's udp buffer then loads the given message reference. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType LoadUDPPairingOkMessage(NetworkEntity sender, ref Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            ServerSideClient ssClientReference = (ServerSideClient)sender;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;///4 bytes reserver to the message length.

            ///Writing the Command byte.
            ssClientReference.udpCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairingOk;
            bytesToSend += 1;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writint the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, ssClientReference.udpCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            System.Buffer.BlockCopy(ssClientReference.udpCollection.rawBuffer, 0, targetMessage.bodyMessage, 0, bytesToSend);
            targetMessage.messageRawLength = (uint)bytesToSend;
            targetMessage.command = (Message.CommandType)ssClientReference.udpCollection.rawBuffer[PacketHandler.PrefixSize + 4];
            //targetMessage = new RawMessage((CommandType)ssClientReference.udpCollection.rawBuffer[Message.HeaderOfMessageCommand.Length + 4], ssClientReference.udpCollection.rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /*
        /// <summary>
        /// Writes an UDPParingFailMessage message in a raw format into the sender's udp buffer then creates a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType UDPPairingFailMessage(NetworkEntity sender, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            ServerSideClient ssClientReference = (ServerSideClient)sender;
            targetMessage = null;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            ssClientReference.udpCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairingFail;
            bytesToSend += 1;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, ssClientReference.udpCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new RawMessage((CommandType)ssClientReference.udpCollection.rawBuffer[Message.HeaderOfMessageCommand.Length + 8], ssClientReference.udpCollection.rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }
        */

        /// <summary>
        /// Writes an UDPParingFailMessage message in a raw format into the sender's udp buffer then loads a Message object. <b>The previous content is discarded.</b>
        /// </summary>
        /// <param name="sender">Reference to sender that holds the buffer to write in.</param>
        /// <param name="targetMessage">Out reference to the Message object to be created.</param>
        /// <returns></returns>
        public static Error.ErrorType LoadUDPPairingFailMessage(NetworkEntity sender, ref Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            ServerSideClient ssClientReference = (ServerSideClient)sender;
            byte[] messageHeaderContent = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;///4 bytes reserved to write the message length.

            ///Writing the Command byte.
            ssClientReference.udpCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.UDPPairingFail;
            bytesToSend += 1;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, ssClientReference.udpCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Loading the content to the targetMessage
            System.Buffer.BlockCopy(ssClientReference.udpCollection.rawBuffer, 0, targetMessage.bodyMessage, 0, bytesToSend);
            targetMessage.messageRawLength = (uint)bytesToSend;
            targetMessage.command = (Message.CommandType)ssClientReference.udpCollection.rawBuffer[PacketHandler.PrefixSize + 4];
            
            return Error.ErrorType.Ok;
        }

        #endregion

        #region UserCommands

        /// <summary>
        /// Creates an empty User command, it must be sent using the TCP channel.
        /// </summary>
        /// <param name="sender">NetworkEntity</param>
        /// <param name="targetIds">Flag with the clients ids.</param>
        /// <param name="targetMessage"></param>
        /// <returns></returns>
        public static Error.ErrorType EmptyUserCommandMessage(NetworkEntity sender, int targetIds, out Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            byte[] rawBuffer = new byte[ServerSettings.ServerBufferSize];
            targetMessage = null;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;

            ///Writing the Command byte.
            rawBuffer[bytesToSend] = (byte)Message.CommandType.User;
            bytesToSend += 1;

            ///Writing the UserCommand following the Command itself. By default it is written a 0 value to achieve the empty message.
            rawBuffer[bytesToSend] = 255;
            bytesToSend += 1;

            ///Writing the targets ids
            byteBuffer = System.BitConverter.GetBytes( targetIds);
            System.Buffer.BlockCopy(byteBuffer, 0, rawBuffer, bytesToSend, 4);
            bytesToSend += 4;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);
            targetMessage = new ManagedMessage((CommandType)rawBuffer[Message.HeaderOfMessageCommand.Length + 8], sender);
            targetMessage.SetBodyMessage(rawBuffer, (uint)bytesToSend);
            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Loads an empty user command into a RawMessage. <b>It must be sent through the UDP channel.</b>
        /// </summary>
        /// <param name="sender">ServerSideClient reference passed as NetworkEntity.<b>It must be an ServerSideClient reference or an error will be thrown.</b></param>
        /// <param name="targetsIds">Flag with the ids of those clients that shouls receive the message.</param>
        /// <param name="targetMessage">A message loaded with the information.</param>
        /// <returns></returns>
        public static Error.ErrorType LoadUDPEmptyUserCommandMessage(NetworkEntity sender, int targetsIds, ref Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            ServerSideClient ssClientReference = (ServerSideClient)sender;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;///4 bytes reserved to write the message length.

            ///Writing the Command byte.
            ssClientReference.udpCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.User;
            bytesToSend += 1;

            ///Writing the UserCommand following the Command itself. By default it is written a 0 value to achieve the empty message.
            ssClientReference.udpCollection.rawBuffer[bytesToSend] = 255;
            bytesToSend += 1;

            ///Writing the targets ids
            byteBuffer = System.BitConverter.GetBytes(targetsIds);
            System.Buffer.BlockCopy(byteBuffer, 0, ssClientReference.udpCollection.rawBuffer, bytesToSend, 4);
            bytesToSend += 4;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, ssClientReference.udpCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, ssClientReference.udpCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Loading the content to the targetMessage
            System.Buffer.BlockCopy(ssClientReference.udpCollection.rawBuffer, 0, targetMessage.bodyMessage, 0, bytesToSend);
            targetMessage.messageRawLength = (uint)bytesToSend;
            targetMessage.command = (Message.CommandType)ssClientReference.udpCollection.rawBuffer[PacketHandler.PrefixSize + 4];

            return Error.ErrorType.Ok;
        }

        /// <summary>
        /// Loads an empty user command into a RawMessage. <b>It must be sent through the UDP channel.</b>
        /// </summary>
        /// <param name="sender">GameClient reference passed as NetworkEntity.<b>It must be an GameClient reference or an error will be thrown.</b></param>
        /// <param name="targetsIds">Flag with the ids of those clients that shouls receive the message.</param>
        /// <param name="targetMessage">A message loaded with the information.</param>
        /// <returns></returns>
        public static Error.ErrorType LoadUDPEmptyUserCommandMessageFromClient(NetworkEntity sender, int targetsIds, ref Message targetMessage)
        {
            int bytesToSend = Message.HeaderOfMessageCommand.Length;
            GameClient gameClientReference = (GameClient)sender;
            byte[] messageHeaderContent = null;
            byte[] byteBuffer = null;
            if (sender == null)
            {
                return Error.ErrorType.InvalidNetworkEntity;
            }

            ///Writing header
            System.Buffer.BlockCopy(Message.HeaderOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, 0, Message.HeaderOfMessageCommand.Length);
            bytesToSend += 8;///4 bytes reserved to write the message length.

            ///Writing the Command byte.
            gameClientReference.udpNetworkCollection.rawBuffer[bytesToSend] = (byte)Message.CommandType.User;
            bytesToSend += 1;

            ///Writing the UserCommand following the Command itself. By default it is written a 0 value to achieve the empty message.
            gameClientReference.udpNetworkCollection.rawBuffer[bytesToSend] = 255;
            bytesToSend += 1;

            ///Writing the targets ids
            byteBuffer = System.BitConverter.GetBytes(targetsIds);
            System.Buffer.BlockCopy(byteBuffer, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, 4);
            bytesToSend += 4;

            ///Writint the EndOfMessageCommand.
            System.Buffer.BlockCopy(Message.EndOfMessageCommand, 0, gameClientReference.udpNetworkCollection.rawBuffer, bytesToSend, Message.EndOfMessageCommand.Length);
            bytesToSend += EndOfMessageCommand.Length;

            ///Writing the message length.
            messageHeaderContent = System.BitConverter.GetBytes(bytesToSend);
            System.Buffer.BlockCopy(messageHeaderContent, 0, gameClientReference.udpNetworkCollection.rawBuffer, Message.HeaderOfMessageCommand.Length, messageHeaderContent.Length);

            ///Loading the content to the targetMessage
            System.Buffer.BlockCopy(gameClientReference.udpNetworkCollection.rawBuffer, 0, targetMessage.bodyMessage, 0, bytesToSend);
            targetMessage.messageRawLength = (uint)bytesToSend;
            targetMessage.command = (Message.CommandType)gameClientReference.udpNetworkCollection.rawBuffer[PacketHandler.PrefixSize + 4];

            return Error.ErrorType.Ok;
        }

        #endregion
    }
}
