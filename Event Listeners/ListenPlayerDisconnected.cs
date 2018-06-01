using Rocket.API.Eventing;
using Rocket.API.Plugins;
using Rocket.Core.Eventing;
using Rocket.Core.Player.Events;

namespace persiafighter.Plugins.Jobs.EventListeners
{
    public class ListenPlayerDisconnected : IEventListener<PlayerDisconnectedEvent>
    {
        private readonly RocketJobs rocketJobs;

        [EventHandler]
        public void HandleEvent(IEventEmitter emitter, PlayerDisconnectedEvent @event) => rocketJobs.Helper.PlayerDisconnected(@event.Player.Id);

        public ListenPlayerDisconnected(IPlugin plugin) => rocketJobs = (RocketJobs)plugin;
    }
}
