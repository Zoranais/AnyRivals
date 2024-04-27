import { QuestionType } from './enums/question-type';

export interface Round {
  answers: string[];
  source: string;
  title?: string;
  questionType: QuestionType;
}
