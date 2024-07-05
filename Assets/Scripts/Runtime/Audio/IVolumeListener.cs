using System.Collections.Generic;

namespace MP.Audio
{
    public interface IVolumeListener
    {
        LinkedListNode<IVolumeListener> RegistryNode { get; set; }
        void OnVolumeChanged(AudioCategory audioCategory, float oldVolume, float newVolume);
    }
}