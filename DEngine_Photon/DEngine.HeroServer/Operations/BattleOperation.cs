using DEngine.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using UnityEngine;

namespace DEngine.HeroServer.Operations
{
    public class BattleOperation : Operation
    {
        public BattleOperation()
        {
        }

        public BattleOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

       
        [DataMember(Code = (byte)ParameterCode.ClientId, IsOptional = false)]
        public Guid ClientId { get; set; }

       
        [DataMember(Code = (byte)ParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }

        [DataMember(Code = (byte)ParameterCode.SubCode, IsOptional = false)]
        public byte SubCode { get; set; }

        [DataMember(Code = (byte)ParameterCode.RoleId, IsOptional = true)]
        public int RoleId { get; set; }

        [DataMember(Code = (byte)ParameterCode.SkillId, IsOptional = true)]
        public int SkillId { get; set; }

        [DataMember(Code = (byte)ParameterCode.TargetId, IsOptional = true)]
        public int TargetId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Difficulty, IsOptional = true)]
        public int Difficulty { get; set; }

        [DataMember(Code = (byte)ParameterCode.TargetPos, IsOptional = true)]
        public float[] TargetPos { get; set; }
    }
}
