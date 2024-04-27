import { GameState } from './enums/game-state';
import { Player } from './player';

export interface Game {
  id: number;
  externalId: string;
  name: string;
  requirePassword: boolean;
  roundCount: number;

  players: Player[];
  state: GameState;
}
