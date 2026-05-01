# Student Wallet Frontend

A modern React TypeScript frontend for the Student Digital Wallet API System.

## Features

### 🔐 Authentication
- Student login with Student ID and PIN
- Demo accounts for testing
- Secure session management with localStorage
- Account lockout handling

### 💰 Wallet Management
- Real-time balance display
- Account profile information
- Balance refresh functionality

### 💸 Transactions
- **Deposit Money**: Add funds to wallet
- **Pay for Services**: Pay for cafeteria, printing, transport, and other services
- **Transfer Money**: Send money to other students with receiver validation
- Real-time balance updates after transactions

### 📊 Transaction History
- Paginated transaction history
- Transaction filtering and search
- Detailed transaction information
- Summary statistics

### 🎨 Modern UI/UX
- Responsive design for mobile and desktop
- Clean, intuitive interface
- Real-time feedback and validation
- Loading states and error handling

## Getting Started

### Prerequisites
- Node.js 16+ and npm
- Student Wallet API running on `http://localhost:5155`

### Installation

1. **Install dependencies**
   ```bash
   npm install
   ```

2. **Start the development server**
   ```bash
   npm start
   ```

3. **Open your browser**
   Navigate to `http://localhost:3000`

## Demo Accounts

The frontend comes with quick-access demo accounts:

| Student ID | Name | PIN | Initial Balance |
|------------|------|-----|----------------|
| STU001 | John Doe | 1234 | $100.00 |
| STU002 | Jane Smith | 5678 | $250.50 |
| STU003 | Mike Johnson | 9999 | $75.25 |

## API Integration

The frontend communicates with the Student Wallet API at `http://localhost:5155/api`. Make sure the API is running before starting the frontend.

### API Endpoints Used
- `POST /api/auth/login` - Student authentication
- `GET /api/auth/validate/{studentId}` - Validate receiver for transfers
- `GET /api/wallet/balance/{studentId}` - Get current balance
- `POST /api/wallet/deposit/{studentId}` - Deposit money
- `POST /api/wallet/pay/{studentId}` - Pay for services
- `POST /api/wallet/transfer/{studentId}` - Transfer money
- `GET /api/wallet/history/{studentId}` - Transaction history

## Project Structure

```
src/
├── components/          # React components
│   ├── Login.tsx       # Login page
│   ├── Dashboard.tsx   # Main dashboard
│   ├── TransactionForm.tsx  # Transaction forms
│   └── TransactionHistory.tsx  # History display
├── context/            # React context
│   └── AuthContext.tsx # Authentication state
├── services/           # API services
│   └── api.ts         # API client
├── types/             # TypeScript types
│   └── api.ts         # API type definitions
└── App.tsx            # Main app component
```

## Features in Detail

### Authentication Flow
1. User enters Student ID and PIN
2. Frontend validates input format
3. API call to authenticate user
4. On success, user data stored in context and localStorage
5. Automatic session restoration on page refresh

### Transaction Processing
1. User selects transaction type (deposit/pay/transfer)
2. Form validation for amount and required fields
3. For transfers, real-time receiver validation
4. API call to process transaction
5. Balance update and success feedback
6. Transaction history refresh

### Error Handling
- Network error handling with user-friendly messages
- Form validation with real-time feedback
- API error display with specific error messages
- Loading states during API calls

## Responsive Design

The frontend is fully responsive and works on:
- Desktop computers (1200px+)
- Tablets (768px - 1199px)
- Mobile phones (320px - 767px)

## Technology Stack

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Axios** - HTTP client
- **CSS3** - Styling with Flexbox and Grid
- **Context API** - State management

## Development

### Available Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm test` - Run tests
- `npm run eject` - Eject from Create React App

### Code Style

- TypeScript for type safety
- Functional components with hooks
- CSS modules for component styling
- Consistent naming conventions

## Security Features

- Input validation and sanitization
- Secure API communication
- Session management with automatic cleanup
- PIN masking in forms

## Future Enhancements

- Real-time notifications
- Dark mode support
- Advanced transaction filtering
- Export transaction history
- Multi-language support
- Progressive Web App (PWA) features

## Troubleshooting

### Common Issues

1. **API Connection Error**
   - Ensure the Student Wallet API is running on `http://localhost:5155`
   - Check CORS configuration in the API

2. **Login Issues**
   - Verify Student ID and PIN format
   - Check if account is locked (wait 30 minutes or use unlock endpoint)

3. **Transaction Failures**
   - Ensure sufficient balance for payments and transfers
   - Verify receiver Student ID exists for transfers

### Support

For issues or questions, check the API documentation or contact the development team.