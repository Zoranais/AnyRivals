import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { Round } from '../../models/round';
import { CommonModule } from '@angular/common';
import { AudioService } from '../../services/audio.service';
import { QuestionType } from '../../models/enums/question-type';

@Component({
  selector: 'app-question',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './question.component.html',
  styleUrl: './question.component.sass',
})
export class QuestionComponent implements OnChanges {
  @Input() round: Round;
  @Output() answerSubmitted = new EventEmitter<string>();

  public selectedAnswer: string = undefined;

  constructor(private audioService: AudioService) {}

  ngOnChanges(simpleChanges: SimpleChanges) {
    if (simpleChanges['round'] && this.round) {
      this.audioService.play(this.round.source);
    }
  }

  public submitAnswer(answer: string) {
    this.selectedAnswer = answer;
    this.answerSubmitted.emit(answer);
  }

  public isSourceNeeded() {
    return (
      this.round.questionType == QuestionType.imageQuestion ||
      this.round.questionType === QuestionType.videoQuestion
    );
  }

  public getQuestionTitle() {
    if (this.round.title) return this.round.title;

    console.log(this.round.questionType);

    switch (this.round.questionType) {
      case QuestionType.spotifyMusicGuess:
        return 'Guess the song!';
      case QuestionType.spotifyAuthorGuess:
        return 'Guess an author!';
      case QuestionType.textQuestion:
        return this.round.source;
      default:
        return '';
    }
  }
}
