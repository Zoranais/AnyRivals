import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-join-game',
  standalone: true,
  imports: [MatInputModule, MatButtonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './join-game.component.html',
  styleUrl: './join-game.component.sass',
})
export class JoinGameComponent {
  public gameId: string = '';
  public isExist = false;

  constructor(private router: Router, private gameService: GameService) {}

  public onGameIdInputChange(event: Event) {
    const element = event.target as HTMLInputElement;
    let formattedValue = element.value
      .toUpperCase()
      .replace(/[^a-zA-Z0-9]/g, '');

    if (formattedValue.length >= 4) {
      formattedValue = `${formattedValue.substring(
        0,
        3
      )}-${formattedValue.substring(3, formattedValue.length)}`;
    }

    this.gameId = formattedValue.substring(0, 7);
    element.value = this.gameId;

    if (this.gameId.length === 7) {
      this.gameService
        .isExist(this.gameId)
        .subscribe((value) => (this.isExist = value));
    }
  }

  public validateGameId() {
    return this.isExist && this.gameId.length === 7;
  }

  public join() {
    if (this.validateGameId()) {
      this.router.navigate([`/game/${this.gameId}`]);
    }
  }
}
