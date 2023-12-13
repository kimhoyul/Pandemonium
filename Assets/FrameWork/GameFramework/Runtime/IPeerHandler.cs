using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOONIPLAY
{
    public interface IPeerHandler
    {
        public void SendSelectCharacter(string uid, int charId);
        public void SendEnterSector(string uid, Vector3 position, Quaternion rotation);
        public void SendAction(string uid, Packets.ActionCmd actionCmd, Vector3 position, Quaternion rotation, int plusArrIndex, float currentSpeed);
    }
}
