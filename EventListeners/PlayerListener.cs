using Rocket.API.Eventing;
using Rocket.Core.Eventing;
using Rocket.Core.Player.Events;

namespace persiafighter.Plugins.Jobs.EventListeners
{
    public class PlayerListener : IEventListener<PlayerDisconnectedEvent>
    {
        private readonly RocketJobsPlugin _rocketJobsPlugin;

        [EventHandler]
        public void HandleEvent(IEventEmitter emitter, PlayerDisconnectedEvent @event) => 
            _rocketJobsPlugin.JobManager.HandlePlayerDisconnect(@event.Player.Id);

        public PlayerListener(RocketJobsPlugin plugin) => _rocketJobsPlugin = plugin;
    }
}
