import { Routes } from '@angular/router';
import { AiAssistantComponent } from './components/ai-assistant/ai-assistant';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'ai-assistant',
    pathMatch: 'full'
  },
  {
    path: 'ai-assistant',
    component: AiAssistantComponent
  }
];