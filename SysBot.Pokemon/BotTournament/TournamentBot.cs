﻿using PKHeX.Core;
using SysBot.Base;
using System.Threading;
using System.Threading.Tasks;
using static SysBot.Base.SwitchButton;

namespace SysBot.Pokemon.BotTournament
{
    // Deacting cheats beforehand and selecting the correct language of your Switch in the configuration is important.
    // You must be connected to a lan-play server for anyone to be able to receive your rules.
    public class TournamentBot : PokeRoutineExecutor
    {
        private readonly PokeTradeHub<PK8> Hub;
        private readonly uint _vsMenuNumber = 8;

        public TournamentBot(PokeBotState cfg, PokeTradeHub<PK8> hub) : base(cfg)
        {
            Hub = hub;
        }

        public override async Task MainLoop(CancellationToken token)
        {
            Log("Identifying trainer data of the host console.");
            await IdentifyTrainer(token).ConfigureAwait(false);

            // Shouldn't ever be used while not on overworld.
            Log("Get to the Overworld, if not already.");
            if (!await IsOnOverworld(Hub.Config, token).ConfigureAwait(false))
                await ExitTrade(Hub.Config, true, token).ConfigureAwait(false);

            Log("Go to share the rules for a local tournament.");
            await GoToLocalTournament(token).ConfigureAwait(false);

            if (Hub.Config.Tournament.CreateRulesOnStart)
            {
            }
            else
            {
                await GoToSendingExistingTournament(token).ConfigureAwait(false);
            }

            Log("Switching to Lan Play Mode.");
            await SwitchToLanPlayMode(token).ConfigureAwait(false);

            await Task.Delay(1_000, token).ConfigureAwait(false);

            Log("Start sending the rules.");
            await Click(A, 1_000, token).ConfigureAwait(false);

            Log("Sending the rules.");
            while (!token.IsCancellationRequested && Config.NextRoutineType == PokeRoutineType.TournamentBot)
            { 
                Config.IterateNextRoutine();

                // When sending the rules, pressing A is enough to continue sending rules when they are sent to someone
                await Click(A, 1_000, token).ConfigureAwait(false);
            }
        }

        private async Task GoToSendingExistingTournament(CancellationToken token)
        {
            await Click(A, 1_000, token).ConfigureAwait(false);
            await Click(A, 1_000, token).ConfigureAwait(false);
            await Click(A, 1_000, token).ConfigureAwait(false);
        }

        private async Task GoToLocalTournament(CancellationToken token)
        {
            await Click(X, 1_000, token).ConfigureAwait(false);

            await NavigateToMenuPoint(_vsMenuNumber, token).ConfigureAwait(false);

            await Click(A, 2_000, token).ConfigureAwait(false);
            await Click(DRIGHT, 500, token).ConfigureAwait(false);
            await Click(A, 2_000, token).ConfigureAwait(false);
            await Click(DDOWN, 500, token).ConfigureAwait(false);
            await Click(A, 1_000, token).ConfigureAwait(false);
        }

        private async Task SwitchToLanPlayMode(CancellationToken token)
        {
            await Connection.SendAsync(SwitchCommand.Hold(R, UseCRLF), token).ConfigureAwait(false);
            await Connection.SendAsync(SwitchCommand.Hold(L, UseCRLF), token).ConfigureAwait(false);
            await Connection.SendAsync(SwitchCommand.Hold(DUP, UseCRLF), token).ConfigureAwait(false);
            await Connection.SendAsync(SwitchCommand.Hold(LSTICK, UseCRLF), token).ConfigureAwait(false);

            await Task.Delay(5_000, token).ConfigureAwait(false);

            await Connection.SendAsync(SwitchCommand.Release(R, UseCRLF), token).ConfigureAwait(false);
            await Connection.SendAsync(SwitchCommand.Release(L, UseCRLF), token).ConfigureAwait(false);
            await Connection.SendAsync(SwitchCommand.Release(DUP, UseCRLF), token).ConfigureAwait(false);
            await Connection.SendAsync(SwitchCommand.Release(LSTICK, UseCRLF), token).ConfigureAwait(false);
        }

    }
}
