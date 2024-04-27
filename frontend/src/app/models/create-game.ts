import { GameType } from './enums/game-type';

export interface CreateGame {
  name: string;
  source: string;
  totalRounds: number;
  password?: string;
  gameType: GameType;
}
