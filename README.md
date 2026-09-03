# AI-Enabled Clinical Management System

An AI-enabled clinical management system demonstrating the integration of Generative AI and Retrieval-Augmented Generation (RAG) with a traditional web application.

The system uses Angular for the frontend and ASP.NET Core Web API (.NET 8) for the backend. The AI assistant provides grounded answers using approved clinical policy documents stored in an in-memory Knowledge Base.

## Project Overview

The application provides an AI-powered clinical policy assistant for questions related to:

- Appointment booking
- Appointment cancellation
- Appointment rescheduling
- Late arrival
- No-show policy
- Emergency appointment guidance
- Insurance verification
- Insurance coverage
- Deductibles
- Co-payments
- Pre-authorization
- Non-covered services
- Insurance claims
- Patient responsibility

The AI assistant uses two approved Knowledge Base documents:

- AppointmentPolicy.txt
- HealthInsurancePolicy.txt

The LLM is provided with relevant content retrieved from these approved documents so that responses remain grounded in the available policy information.

## Architecture

Angular AI Assistant

        |

        | POST /api/AI/ask

        v

ASP.NET Core Web API

        |

        v

RAG Orchestration

        |

        +---- Question Embedding

        |

        v

Knowledge Base

        |

        v

Similarity Search

        |

        v

Top-K Relevant Policy Chunks

        |

        v

RAG Context / Prompt

        |

        v

LLM

        |

        v

Grounded Answer + Source

### Knowledge Base Initialization

AppointmentPolicy.txt

HealthInsurancePolicy.txt

          |

          v

Document Loader

          |

          v

Chunking

          |

          v

Embedding Generation

          |

          v

In-Memory Knowledge Base

The policy documents are loaded, chunked, embedded, and stored in memory during application startup. Their embeddings are reused for later questions.

## RAG Retrieval Configuration

- Top-K results: 3
- Minimum relevance score: 0.50

The minimum relevance score is a retrieval threshold used to determine whether retrieved content is relevant enough to be included in the RAG context. It is not an AI accuracy percentage.

## Technologies Used

### Frontend

- Angular
- TypeScript
- HTML
- SCSS
- Angular HttpClient

### Backend

- ASP.NET Core Web API
- .NET 8
- C#
- Swagger / OpenAPI
- HttpClient
- OpenAI Embeddings
- OpenAI Chat Completion / LLM

### RAG

- Document loading
- Document chunking
- Embeddings
- In-memory vector storage
- Cosine similarity
- Top-K retrieval
- RAG prompt construction
- Grounded LLM responses

## Project Structure

AI-Enabled-Clinic-Management/

│

├── Backend/

│   └── AiEnabledClinicManagement-Solution/

│       ├── AiEnabledClinicManagement-Solution.sln

│       └── AiEnabledClinicManagement/

│           ├── Controllers/

│           ├── Documents/

│           │   ├── AppointmentPolicy.txt

│           │   └── HealthInsurancePolicy.txt

│           ├── Models/

│           ├── Services/

│           ├── Program.cs

│           ├── appsettings.json

│           └── ...

│

├── Frontend/

│   └── ClinicalManagementFrontend/

│       ├── src/

│       │   ├── app/

│       │   └── environments/

│       ├── angular.json

│       ├── package.json

│       └── ...

│

├── Screenshots/

│   ├── ...

│

├── README.md

└── .gitignore

## Screenshots

Screenshots of the clinic interface and Clara AI assistant are available in the `Screenshots` folder.

## AI Assistant

The frontend provides a clinic-style floating AI assistant named **Clara**.

Users can:

1. Open the clinical assistant.

2. Enter a question about an approved clinic policy.

3. Send the question to the ASP.NET Core backend.

4. Retrieve relevant policy content using RAG.

5. Receive a grounded answer from the LLM.

6. View the approved policy source used for the response.

## API Endpoints

### AI Assistant

POST /api/AI/ask

Example request:

{
  "question": "What is a deductible?"
}

Example response:

{
  "answer": "A deductible is the amount a patient may need to pay before insurance starts paying for certain covered services.",
  "sources": [
    "HealthInsurancePolicy.txt"
  ]
}

### Document Chunks

GET /api/Documents/chunks/{fileName}

This endpoint can be used to inspect the chunks generated from the approved policy documents.

### Retrieval Testing

GET /api/Documents/search?question=What%20is%20a%20deductible%3F&topK=3

This endpoint is useful for verifying:

- retrieved policy chunks
- source document
- similarity score
- Top-K retrieval

## Example Questions

### Appointment Policy

- How early should I arrive for my appointment?
- How many hours before should I cancel?
- Can I reschedule my appointment?
- What is a no-show?
- What happens if I arrive late?
- What should I do in an emergency?

### Health Insurance Policy

- What is a deductible?
- What is a co-payment?
- What is pre-authorization?
- Who pays for non-covered services?
- Does submitting a claim guarantee payment?
- Does insurance cover every healthcare service?
- When should I provide my insurance information?

## Grounded Responses

The AI assistant is designed to answer using only the approved policy content retrieved from the Knowledge Base.

For questions that are outside the approved policies, the system should not invent or assume unsupported medical or insurance information.

Example out-of-policy question:

What medicine should I take for a headache?

The assistant should state that the approved clinical policies do not contain information about the question instead of generating unsupported medical advice.

## Running the Backend

Navigate to the backend project:

cd Backend\AiEnabledClinicManagement-Solution\AiEnabledClinicManagement

Run the application:

dotnet run

ASP.NET Core will display the HTTPS URL in the terminal.

Example:

https://localhost:7268

Swagger can then be accessed at:

https://localhost:7268/swagger

Before running the backend, configure the required OpenAI settings locally.

Do not commit real API keys to GitHub.

## Running the Frontend

Navigate to the Angular project:

cd Frontend\ClinicalManagementFrontend

Install dependencies:

npm install

Start the Angular development server:

ng serve -o

The application normally runs at:

http://localhost:4200

The frontend communicates with the backend through:

POST /api/AI/ask

The backend base URL is configured using the Angular environment configuration.

## Configuration

The backend uses local configuration for the OpenAI API settings.

Do not place real API keys in source code.

Do not place API keys in Angular frontend code.

The Angular frontend should only contain the backend API URL.

## Security

Sensitive information should remain outside the Git repository.

The .gitignore file excludes:

- local development configuration
- environment files
- API secrets
- Node modules
- .NET build output
- Visual Studio temporary files

Never commit an OpenAI API key to GitHub.

If an API key is accidentally committed, revoke or rotate the key immediately.

## Testing

The system is designed to test the complete RAG pipeline:

User Question

       |

       v

Question Embedding

       |

       v

Similarity Search

       |

       v

Top-K Policy Chunks

       |

       v

RAG Context

       |

       v

LLM

       |

       v

Grounded Answer

       |

       v

Source Document

### Required Test Cases

#### Appointment

1. How early should I arrive for my appointment?

2. How many hours before should I cancel?

3. Can I reschedule my appointment?

4. What is a no-show?

#### Health Insurance

5. What is a deductible?

6. What is a co-payment?

7. What is pre-authorization?

8. Who pays for non-covered services?

9. Does submitting a claim guarantee payment?

#### Out-of-Policy

What medicine should I take for a headache?

The supported questions should return answers based on the approved policy documents.

The out-of-policy question should not generate unsupported medical advice.

## Future Enhancements

Possible future improvements include:

- Integration with an existing Hospital Management System
- Doctor and appointment management integration
- Persistent vector database
- Authentication and authorization
- Conversation history
- Additional approved clinical documents
- Advanced retrieval techniques
- Production deployment
- Improved monitoring and logging

## Author

AI-Enabled Clinical Management System

Built using:

- Angular
- ASP.NET Core Web API
- .NET 8
- C#
- Generative AI
- Retrieval-Augmented Generation (RAG)
- OpenAI