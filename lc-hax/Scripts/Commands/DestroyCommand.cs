using System.Collections;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("destroy")]
internal class DestroyCommand : ICommand {
    IEnumerator DestroyAllItemsAsync(PlayerControllerB player) {
        float currentWeight = player.carryWeight;

        foreach (GrabbableObject grabbable in Helper.Grabbables.ToArray()) {
            player.GrabObject(grabbable);
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == grabbable);
            player.DespawnHeldObject();
        }

        player.carryWeight = currentWeight;
    }

    Result DestroyHeldItem(PlayerControllerB player) {
        if (player.currentlyHeldObjectServer is null) {
            return new Result(message: "You are not holding anything!");
        }

        player.DespawnHeldObject();
        return new Result(true);
    }

    Result DestroyAllItems(PlayerControllerB player) {
        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.DestroyAllItemsAsync(player));

        return new Result(true);
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        Result result = args[0] switch {
            null => this.DestroyHeldItem(player),
            "--all" => this.DestroyAllItems(player),
            _ => new Result(message: "Invalid arguments!")
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
