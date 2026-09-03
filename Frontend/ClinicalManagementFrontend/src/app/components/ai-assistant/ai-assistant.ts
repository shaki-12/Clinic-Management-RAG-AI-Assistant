import { CommonModule } from '@angular/common';
import {
  AfterViewChecked,
  Component,
  ElementRef,
  ViewChild,
  computed,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AiService } from '../../services/ai';
import { AskRequest } from '../../models/ask-request';
import { AskResponse } from '../../models/ask-response';

interface ChatMessage {
  id: number;
  role: 'user' | 'bot';
  text: string;
  sources: string[];
  isError: boolean;
}

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-assistant.html',
  styleUrl: './ai-assistant.scss'
})
export class AiAssistantComponent implements AfterViewChecked {

  @ViewChild('messagesEnd') private messagesEnd?: ElementRef<HTMLDivElement>;
  @ViewChild('composer') private composer?: ElementRef<HTMLTextAreaElement>;

  /** Whether the widget is expanded into a full chat panel. */
  isOpen = signal<boolean>(false);

  question = signal<string>('');

  messages = signal<ChatMessage[]>([]);

  isLoading = signal<boolean>(false);

  errorMessage = signal<string | null>(null);

  hasAsked = computed(() => this.messages().length > 0);

  readonly suggestedQuestions: string[] = [
    'How early should I arrive for my appointment?',
    'How much notice is needed to cancel my appointment?',
    'Can I reschedule my appointment?',
    'What is a deductible?'
  ];

  private messageIdCounter = 0;
  private pendingScroll = false;

  constructor(private aiService: AiService) {}

  ngAfterViewChecked(): void {
    if (this.pendingScroll) {
      this.scrollToBottom();
      this.pendingScroll = false;
    }
  }

  toggleWidget(): void {
    this.isOpen.update((open) => !open);

    if (this.isOpen()) {
      this.pendingScroll = true;
      setTimeout(() => this.composer?.nativeElement.focus(), 250);
    }
  }

  closeWidget(): void {
    this.isOpen.set(false);
  }

  askQuestion(): void {
    const trimmedQuestion = this.question().trim();

    if (!trimmedQuestion) {
      this.errorMessage.set('Please enter a question first.');
      return;
    }

    if (this.isLoading()) {
      return;
    }

    this.errorMessage.set(null);
    this.pushMessage('user', trimmedQuestion);
    this.question.set('');
    this.isLoading.set(true);
    this.pendingScroll = true;

    const request: AskRequest = {
      question: trimmedQuestion
    };

    this.aiService.ask(request).subscribe({
      next: (response: AskResponse) => {
        this.pushMessage(
          'bot',
          response.answer ?? "I couldn't find an answer for that — try rephrasing your question.",
          response.sources ?? []
        );
        this.isLoading.set(false);
        this.pendingScroll = true;
      },

      error: (error) => {
        console.error('AI Assistant Error:', error);

        this.pushMessage(
          'bot',
          'Unable to connect to the clinical AI assistant. Please try again.',
          [],
          true
        );

        this.errorMessage.set(
          'Unable to connect to the clinical AI assistant. Please try again.'
        );

        this.isLoading.set(false);
        this.pendingScroll = true;
      }
    });
  }

  useSuggestedQuestion(question: string): void {
    this.question.set(question);
    this.errorMessage.set(null);
    this.askQuestion();
  }

  clearChat(): void {
    this.messages.set([]);
    this.question.set('');
    this.errorMessage.set(null);
    this.isLoading.set(false);
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.askQuestion();
    }
  }

  onComposerInput(value: string): void {
    this.question.set(value);
    if (this.errorMessage()) {
      this.errorMessage.set(null);
    }
  }

  get characterCount(): number {
    return this.question().length;
  }

  private pushMessage(
    role: 'user' | 'bot',
    text: string,
    sources: string[] = [],
    isError = false
  ): void {
    this.messages.update((list) => [
      ...list,
      { id: ++this.messageIdCounter, role, text, sources, isError }
    ]);
  }

  private scrollToBottom(): void {
    try {
      this.messagesEnd?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
    } catch {
      /* no-op: scrolling is a nicety, never block the flow on it */
    }
  }
}
