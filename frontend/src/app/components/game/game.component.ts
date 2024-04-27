import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { ActivatedRoute, Router } from '@angular/router';
import { Game } from '../../models/game';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { Player } from '../../models/player';
import { PlayerListComponent } from '../player-list/player-list.component';
import { Round } from '../../models/round';
import { QuestionComponent } from '../question/question.component';
import { GameState } from '../../models/enums/game-state';
import { CommonModule } from '@angular/common';
import { GameService } from '../../services/game.service';
import { AudioService } from '../../services/audio.service';
import { Answer } from '../../models/answer';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressSpinnerModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
    PlayerListComponent,
    QuestionComponent,
  ],
  templateUrl: './game.component.html',
  styleUrl: './game.component.sass',
})
export class GameComponent implements OnInit, OnDestroy {
  public isLoading = true;
  public gameId: string;
  public name = '';
  public game: Game | undefined = undefined;
  public round: Round | undefined = undefined;
  public correctAnswers: Answer[] = [];
  public winner: Player = undefined;

  //states
  public isJoined = false;
  public isDistributing = false;
  public isRevealing = false;

  private connection: signalR.HubConnection;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private gameService: GameService,
    private audioService: AudioService
  ) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/GameHub`, {})
      .build();

    this.gameId = route.snapshot.params['gameId'];
  }

  public isWaiting() {
    return this.game.state === GameState.waiting;
  }
  public isRunning() {
    return this.game.state === GameState.running;
  }
  public isEnded() {
    return this.game.state === GameState.ended;
  }
  public isPreparing() {
    return this.game.state === GameState.initializing;
  }

  public get correctAnswer() {
    if (!this.correctAnswers || this.correctAnswers.length == 0)
      return undefined;

    return this.correctAnswers
      .map(
        (x) =>
          `${x.text} ${
            x.correctnessCoefficient !== 1
              ? '(' + x.correctnessCoefficient + ')'
              : ''
          }`
      )
      .join(', ');
  }

  ngOnInit(): void {
    this.gameService.isExist(this.gameId).subscribe((exist) => {
      if (!exist) {
        this.router.navigate(['/']);
      }
    });

    this.name = localStorage.getItem('username') ?? '';

    this.connection
      .start()
      .then(() => (this.isLoading = false))
      .catch((err) => {
        console.log('Error while starting connection: ' + err);
        this.router.navigate(['']);
      });

    this.connection.on('Connected', (game: Game) => {
      this.connected(game);
    });
    this.connection.on('DistributeQuestion', (round: Round) => {
      this.questionDistributed(round);
    });
    this.connection.on(
      'RevealAnswer',
      (correctAnswers: Answer[], players: Player[]) => {
        this.answerRevealed(correctAnswers, players);
      }
    );
    this.connection.on('PlayerJoined', (player: Player) => {
      this.playerJoined(player);
    });
    this.connection.on('PlayerDisconnected', (player: Player) => {
      this.playerLeft(player);
    });
    this.connection.on('GameStarting', () => this.gameStarting());
    this.connection.on('GameEnded', () => this.gameEnded());
    this.connection.on('GameInitialized', () => this.gameInitialized());
  }

  public validateName() {
    return this.name.length >= 3;
  }

  //invoke methods
  public join() {
    this.isLoading = true;
    localStorage.setItem('username', this.name);
    this.connection
      .invoke('Join', this.gameId, this.name)
      .then(() => {
        console.log('Join Executed');
        this.isLoading = false;
      })
      .catch((err) => {
        console.log('Error while invoking: ' + err);
      });
  }

  public respond(answer: string) {
    this.connection
      .invoke('Respond', answer)
      .then(() => console.log('Respond Executed'))
      .catch((err) => {
        console.log('Error while invoking: ' + err);
      });
  }

  public startGame() {
    if (this.game.players.length < 2) {
      return;
    }
    this.connection
      .invoke('StartGame')
      .then(() => {
        console.log('StartGame Executed');
        this.game.state = GameState.running;
      })
      .catch((err) => {
        console.log('Error while invoking: ' + err);
      });
  }

  //server-side methods
  public connected(game: Game) {
    this.game = game;
    this.isJoined = true;
    console.log('Connected to: ', game);
  }

  public gameInitialized() {
    this.game.state = GameState.waiting;
  }

  public gameStarting() {
    this.game.state = GameState.running;
  }

  public questionDistributed(round: Round) {
    this.round = round;

    this.isRevealing = false;
    this.isDistributing = true;
  }

  public answerRevealed(correctAnswers: Answer[], players: Player[]) {
    this.isDistributing = false;
    this.isRevealing = true;

    this.game.players = players;
    this.correctAnswers = correctAnswers;
  }

  public gameEnded() {
    this.isDistributing = false;
    this.isRevealing = false;

    this.game.state = GameState.ended;
    this.winner = this.game.players.sort((a, b) => b.score - a.score)[0];
  }

  public playerJoined(player: Player) {
    if (this.game) {
      this.game.players = this.game.players.concat(player);
      console.log('Player joined: ', player);
    }
  }

  public playerLeft(player: Player) {
    if (this.game) {
      this.game.players = this.game.players.filter(
        (x) => x.name != player.name
      );
      console.log('Player left: ', player);
    }
  }

  ngOnDestroy(): void {
    this.connection.stop();
    this.audioService.stop();
  }
}
