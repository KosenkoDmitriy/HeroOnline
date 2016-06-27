using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;

namespace DEngine.HeroServer.Operations
{
    public class UserUpdateOperation : Operation
    {
        public UserUpdateOperation()
        {
        }

        public UserUpdateOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ParameterCode.SubCode, IsOptional = false)]
        public byte SubCode { get; set; }

        [DataMember(Code = (byte)ParameterCode.AvatarId, IsOptional = true)]
        public int AvatarId { get; set; }

        [DataMember(Code = (byte)ParameterCode.RoleId, IsOptional = true)]
        public int RoleUId { get; set; }

        [DataMember(Code = (byte)ParameterCode.UserRoles, IsOptional = true)]
        public int[] UserRoles { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = true)]
        public int ItemUId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemCount, IsOptional = true)]
        public int ItemCount { get; set; }

        [DataMember(Code = (byte)ParameterCode.FriendName, IsOptional = true)]
        public string FriendName { get; set; }

        [DataMember(Code = (byte)ParameterCode.FriendMode, IsOptional = true)]
        public int FriendMode { get; set; }

        [DataMember(Code = (byte)ParameterCode.TargetId, IsOptional = true)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param01, IsOptional = true)]
        public int Param01 { get; set; }

        [DataMember(Code = (byte)ParameterCode.Param02, IsOptional = true)]
        public int Param02 { get; set; }
    }
}
