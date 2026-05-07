# Student Digital Wallet API System

A comprehensive digital wallet system for university students built with ASP.NET Core Web API. Students can store money, pay for services, and transfer funds to other students, similar to an ATM system.

## Features

### 🔐 Authentication & Security
- Student login with Student ID and PIN
- Account lockout after 3 failed login attempts (30-minute lockout)
- PIN validation and security measures

### 💰 Wallet Management
- Check account balance and profile
- View wallet information and transaction history

### 💸 Transactions
- **Deposit Money**: Add funds to wallet
- **Pay for Services**: Pay for cafeteria, printing, transport, and other services
- **Transfer Money**: Send money to other students
- **Business Rules**: No negative balance, receiver validation for transfers

### 📊 Reporting
- Individual transaction history with pagination
- Daily transaction summaries
- Overall system statistics (deposits vs payments)

## API Endpoints

### Authentication
- `POST /api/auth/login` - Student login
- `GET /api/auth/validate/{studentId}` - Validate student exists
- `POST /api/auth/unlock/{studentId}` - Unlock locked account (admin)

### Wallet Operations
- `GET /api/wallet/balance/{studentId}` - Get balance and profile
- `POST /api/wallet/deposit/{studentId}` - Deposit money
- `POST /api/wallet/pay/{studentId}` - Pay for services
- `POST /api/wallet/transfer/{studentId}` - Transfer to another student
- `GET /api/wallet/history/{studentId}` - Get transaction history

### Reports
- `GET /api/reports/daily` - Today's transaction summary
- `GET /api/reports/daily/{date}` - Specific date summary
- `GET /api/reports/summary` - Overall system summary

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Installation

1. **Clone and navigate to the project**
   ```bash
   cd StudentWalletAPI
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string** (if needed)
   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentWalletDB;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Open browser and go to: `https://localhost:7000` (or the port shown in console)

## Transaction Flow Example

1. **Login**
   ```json
   POST /api/auth/login
   {
     "studentId": "STU001",
     "pin": "1234"
   }
   ```

2. **Check Balance**
   ```json
   GET /api/wallet/balance/STU001
   ```

3. **Deposit Money**
   ```json
   POST /api/wallet/deposit/STU001
   {
     "amount": 50.00,
     "description": "Monthly allowance"
   }
   ```

4. **Pay for Service**
   ```json
   POST /api/wallet/pay/STU001
   {
     "amount": 15.50,
     "serviceType": 0,
     "description": "Lunch at cafeteria"
   }
   ```

5. **Transfer to Friend**
   ```json
   POST /api/wallet/transfer/STU001
   {
     "receiverStudentId": "STU002",
     "amount": 25.00,
     "description": "Shared textbook cost"
   }
   ```

## Service Types

- `0` - Cafeteria
- `1` - Printing
- `2` - Transport
- `3` - Other

## Security Features

- **PIN Protection**: 4-digit PIN required for login
- **Account Lockout**: Automatic lockout after 3 failed attempts
- **Transaction Validation**: Balance checks and receiver validation
- **Database Transactions**: Atomic operations for data consistency

## Error Handling

The API returns consistent error responses:

```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": ["Detailed error messages"]
}
```

## Testing

Use the included `TestClient.http` file with REST Client extension in VS Code, or test via Swagger UI at the root URL.

## Database Schema

### Students Table
- Id, StudentId (unique), Name, PIN, Balance
- FailedLoginAttempts, IsLocked, LockedUntil
- CreatedAt, UpdatedAt

### Transactions Table
- Id, TransactionId (unique), StudentId, Type, Amount
- BalanceAfter, Description, ServiceType
- ReceiverStudentId, ReceiverStudentNumber, CreatedAt

## Architecture

- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic and transaction processing
- **Models**: Data entities and DTOs
- **Data**: Entity Framework DbContext and database operations

## Future Enhancements

- JWT authentication for session management
- Real-time notifications for transactions
- Mobile app integration
- Advanced reporting and analytics
- Multi-currency support
- Transaction limits and spending controls
