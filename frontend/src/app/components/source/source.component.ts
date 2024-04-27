import { Component, Input } from '@angular/core';
import { QuestionType } from '../../models/enums/question-type';
import { VolumeService } from '../../services/volume.service';

@Component({
  selector: 'app-source',
  standalone: true,
  imports: [],
  templateUrl: './source.component.html',
  styleUrl: './source.component.sass',
})
export class SourceComponent {
  @Input() type: QuestionType;
  @Input() source: string;

  constructor(private volumeService: VolumeService) {}

  get isVideo() {
    return this.type === QuestionType.videoQuestion;
  }

  get isImage() {
    return this.type === QuestionType.imageQuestion;
  }

  get volume() {
    return this.volumeService.getVolume();
  }
}
