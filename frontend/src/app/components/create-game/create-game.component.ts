import { Component, OnDestroy } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CreateGame } from '../../models/create-game';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Subject, takeUntil } from 'rxjs';
import { Router } from '@angular/router';
import { GameType } from '../../models/enums/game-type';

@Component({
  selector: 'app-create-game',
  standalone: true,
  imports: [
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  templateUrl: './create-game.component.html',
  styleUrl: './create-game.component.sass',
})
export class CreateGameComponent implements OnDestroy {
  public form: FormGroup;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private httpClient: HttpClient,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.min(3), Validators.max(36)]],
      spotifyUrl: [
        '',
        [
          Validators.required,
          Validators.pattern(/^https:\/\/open\.spotify\.com\/playlist\//g),
        ],
      ],
      rounds: [3, [Validators.required, Validators.min(3), Validators.max(36)]],
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const formData = this.form.value;
      const dto = {
        name: formData.name,
        source: formData.spotifyUrl,
        totalRounds: formData.rounds,
        password: undefined,
        gameType: GameType.mixedGuess,
      } as CreateGame;

      console.log(dto);
      var response = this.httpClient.post(
        `${environment.apiUrl}/api/game`,
        dto
      );
      response.pipe(takeUntil(this.unsubscribe$)).subscribe(
        (gameId) => {
          this.router.navigate([`/game/${gameId}`]);
        },
        (error) => console.log(error)
      );
    }
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
