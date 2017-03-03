using LiteNetLib.Utils;

namespace Game.Network {
    public interface IEncodable {
        void Serialize(NetDataWriter writer);
        void Deserialize(NetDataReader reader);
    }
}
