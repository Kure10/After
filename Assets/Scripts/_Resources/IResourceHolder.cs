using static ResourceManager;

namespace Resources
{
    public interface IResourceHolder
    {
        ResourceAmount Amount { get; set; }
        void Set(ResourceAmount amount);
        ResourceAmount Add(ResourceAmount amount);
        ResourceAmount Remove(ResourceAmount amount);
    }
}