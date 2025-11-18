# Computor V2 Project Plan

## 1. Approach to the Project

### 1.1 Code Reuse Analysis from Computor V1

**What we can reuse (60-70% of existing code):**
- **Core mathematical foundation**: Custom square root and absolute value implementations (`CustomMath.cs`)
- **Polynomial parsing patterns**: Regular expressions and term extraction logic from `PolynomialParser.cs`
- **Input validation structure**: Character validation and syntax checking from `InputHandler.cs`
- **Error handling patterns**: Comprehensive error messages and validation approach
- **Output formatting concepts**: Pretty-printing and step-by-step solution display from `OutputHandler.cs`
- **Project structure**: Clean separation of Core, IO, and main program logic
- **Graph visualization**: Text-based polynomial graphing from `CustomMath.cs`

**What needs complete rewrite (30-40%):**
- **Type system**: Need to create new types (Complex, Matrix, Function, Rational)
- **Expression evaluator**: Interactive interpreter vs single equation solver
- **Variable management**: Storage and retrieval of variables across sessions
- **Multi-type operations**: Operations between different mathematical types
- **Interactive shell**: REPL (Read-Eval-Print Loop) instead of single-shot execution

### 1.2 Migration Strategy
1. **Keep the foundation**: Preserve the solid mathematical core and project architecture
2. **Extend incrementally**: Build new type system on top of existing polynomial foundation
3. **Refactor for interactivity**: Transform from single-shot solver to interactive interpreter
4. **Maintain quality**: Keep the excellent error handling and user experience standards

## 2. General Project Structure

```
ComputorV2/
├── ComputorV2.csproj
├── Program.cs                          # Main entry point with REPL
├── README.md
├── Core/
│   ├── Types/
│   │   ├── IRationalNumber.cs          # Base interface
│   │   ├── RationalNumber.cs           # Q numbers
│   │   ├── ComplexNumber.cs            # Complex with rational coefficients
│   │   ├── Matrix.cs                   # Mn,p(Q) matrices
│   │   ├── Function.cs                 # Single-variable functions
│   │   └── TypeChecker.cs              # Type inference and validation
│   ├── Math/
│   │   ├── CustomMath.cs               # Extended from V1
│   │   ├── Operations.cs               # Cross-type operations
│   │   ├── MatrixOperations.cs         # Matrix-specific operations
│   │   └── PolynomialSolver.cs         # Enhanced from V1
│   ├── Parsing/
│   │   ├── Lexer.cs                    # Token analysis
│   │   ├── Parser.cs                   # Expression parsing
│   │   ├── ExpressionEvaluator.cs      # Evaluation engine
│   │   └── SyntaxValidator.cs          # Extended from V1
│   └── Variables/
│       ├── Variable.cs                 # Variable storage
│       ├── VariableManager.cs          # Variable lifecycle
│       └── TypeInference.cs            # Dynamic typing
├── IO/
│   ├── InputHandler.cs                 # Enhanced from V1
│   ├── OutputFormatter.cs              # Enhanced from V1
│   ├── CommandParser.cs                # Command interpretation
│   └── DisplayManager.cs               # Multi-type display
├── Interactive/
│   ├── REPL.cs                         # Read-Eval-Print Loop
│   ├── CommandProcessor.cs             # Command routing
│   ├── HistoryManager.cs               # Command history (bonus)
│   └── HelpSystem.cs                   # Built-in help
└── Tests/
    ├── UnitTests/
    ├── IntegrationTests/
    └── TestData/
```

## 3. Development Phases

### Phase 1: Foundation (Days 1-3)
**Objective**: Set up core infrastructure and basic types
- [ ] Create project structure
- [ ] Implement `RationalNumber` class with basic operations
- [ ] Set up basic REPL shell
- [ ] Implement variable assignment for rational numbers
- [ ] Port and enhance input validation from V1

**Deliverables**:
```bash
> varA = 2
2
> varB = 4.242
4.242
> varA + varB = ?
6.242
```

### Phase 2: Complex Numbers (Days 4-5)
**Objective**: Add imaginary number support
- [ ] Implement `ComplexNumber` class
- [ ] Add complex number parsing (2*i + 3)
- [ ] Implement complex arithmetic operations
- [ ] Update type inference system

**Deliverables**:
```bash
> varA = 2*i + 3
3 + 2i
> varB = -4i - 4
-4 - 4i
> varA * varB = ?
4 - 20i
```

### Phase 3: Matrix Support (Days 6-8)
**Objective**: Full matrix functionality
- [ ] Implement `Matrix` class with validation
- [ ] Add matrix parsing ([[2,3];[4,3]])
- [ ] Implement matrix operations (+, -, *, **)
- [ ] Add matrix-scalar operations

**Deliverables**:
```bash
> matA = [[2,3];[4,3]]
[ 2 , 3 ]
[ 4 , 3 ]
> matB = [[1,0];[0,1]]
[ 1 , 0 ]
[ 0 , 1 ]
> matA ** matB = ?
[ 2 , 3 ]
[ 4 , 3 ]
```

### Phase 4: Functions and Equations (Days 9-11)
**Objective**: Function support and polynomial solving
- [ ] Implement `Function` class
- [ ] Add function parsing (funA(x) = 2*x + 1)
- [ ] Function evaluation and composition
- [ ] Integrate polynomial solver from V1
- [ ] Equation solving interface

**Deliverables**:
```bash
> funA(x) = 2*x + 1
2 * x + 1
> funA(5) = ?
11
> funA(x) = 0 ?
x^2 + 2x + 1 = 0
Solution: x = -1
```

### Phase 5: Cross-Type Operations (Days 12-13)
**Objective**: Enable operations between different types
- [ ] Implement automatic type promotion
- [ ] Cross-type arithmetic (rational + complex, matrix * scalar)
- [ ] Type compatibility checking
- [ ] Expression simplification

### Phase 6: Polish and Testing (Days 14-15)
**Objective**: Bug fixes, performance, and user experience
- [ ] Comprehensive testing suite
- [ ] Error message improvements
- [ ] Performance optimization
- [ ] Documentation completion

## 4. Roadmap (Mandatory + Bonus)

### Mandatory Features ✅
1. **Mathematical Types Support**
   - [x] Rational numbers (Q)
   - [x] Complex numbers (a + ib, (a,b) ∈ Q²)
   - [x] Matrices (Mn,p(Q))
   - [x] Functions (single variable)

2. **Assignment Operations**
   - [x] Variable assignment by type inference
   - [x] Variable reassignment with type change
   - [x] Variable-to-variable assignment

3. **Computational Features**
   - [x] Four basic operations (+, -, *, /)
   - [x] Modulo operator (%)
   - [x] Matrix multiplication (**)
   - [x] Power operator (^)
   - [x] Parentheses and operator precedence
   - [x] Expression evaluation with ? operator

4. **Equation Solving**
   - [x] Polynomial equations ≤ degree 2
   - [x] Function evaluation
   - [x] Variable substitution

5. **Program Control**
   - [x] Interactive shell/interpreter
   - [x] Exit functionality
   - [x] Syntax validation

### Selected Bonus Features 🌟
Based on implementation complexity vs. impact analysis:

1. **Function Composition** (Medium complexity, High impact)
   ```bash
   > funA(x) = 2*x + 1
   > funB(x) = x^2
   > funA(funB(x)) = ?
   2 * x^2 + 1
   ```

2. **Variable and Function Listing** (Low complexity, High impact)
   ```bash
   > list
   Variables:
   varA = 3 + 2i
   varB = [[1,2];[3,4]]
   Functions:
   funA(x) = 2*x + 1
   ```

3. **Command History** (Low complexity, Medium impact)
   ```bash
   > history
   1. varA = 2 + 3*i
   2. varB = [[1,2];[3,4]]
   3. varA + 5 = ?
   > !1
   varA = 2 + 3*i
   ```

4. **Matrix Inversion** (Medium complexity, Medium impact)
   ```bash
   > matA = [[2,1];[1,1]]
   > inv(matA) = ?
   [ 1 , -1 ]
   [ -1 , 2 ]
   ```

5. **Extended Mathematical Functions** (Medium complexity, High impact)
   ```bash
   > sqrt(16) = ?
   4
   > sin(0) = ?
   0
   > abs(-5 + 3*i) = ?
   5.83095...
   ```

**Bonus features NOT selected** (due to time constraints):
- Function curve display (High complexity)
- Vector computation extension (Medium complexity, Low priority)
- Radian computation for angles (Low impact for core functionality)
- Norm computation (Lower priority than selected features)

## 5. First Day Approach

### Day 1 Session Plan (6-8 hours)

**Hour 1-2: Project Setup and Analysis**
1. Create new C# project structure
2. Copy reusable components from computor v1:
   - `CustomMath.cs` → enhance for new types
   - Input validation patterns from `InputHandler.cs`
   - Project organization approach
3. Set up Git repository and initial commit

**Hour 3-4: Core Type System Foundation**
1. Implement `IRationalNumber` interface
2. Create `RationalNumber` class with:
   - Fraction representation (numerator/denominator)
   - Basic arithmetic operations
   - GCD/LCM utilities for fraction simplification
   - Conversion from/to decimal

**Hour 5-6: Basic REPL and Variable Management**
1. Create basic `REPL.cs` for interactive shell
2. Implement `Variable.cs` for storing typed values
3. Create `VariableManager.cs` for variable storage/retrieval
4. Basic command parsing for assignment (varA = 2)

**Hour 7-8: Integration and Testing**
1. Wire up components for basic rational number assignment
2. Test basic functionality:
   ```bash
   > varA = 2
   2
   > varB = 3.5
   7/2
   > varA + varB = ?
   7/2
   ```
3. Set up basic error handling and user feedback

**End of Day 1 Goal**: Have a working calculator that can handle rational number assignment, basic arithmetic, and expression evaluation.

**Success Criteria**:
- ✅ User can assign rational numbers to variables
- ✅ User can perform basic arithmetic with rational numbers
- ✅ System displays results in simplified fraction format when appropriate
- ✅ Basic error handling for invalid input
- ✅ Clean exit functionality

## 6. Selected Bonus Features (Detailed)

### 6.1 Function Composition
**Implementation Time**: 2-3 days
**Value**: High - Demonstrates advanced mathematical concepts

**Technical Approach**:
- Extend `Function` class with composition operator
- Implement symbolic substitution algorithm
- Handle nested function calls with proper variable scoping

**Example Implementation**:
```csharp
public Function Compose(Function other, string variable)
{
    // Replace variable in this function with other function's expression
    return new Function(this.Variable, SubstituteExpression(this.Expression, variable, other.Expression));
}
```

### 6.2 Variable and Function Listing  
**Implementation Time**: 1 day
**Value**: High - Essential for user experience in interactive environment

**Technical Approach**:
- Add `list` command to command parser
- Iterate through `VariableManager` storage
- Format output with type information and current values

### 6.3 Command History
**Implementation Time**: 1-2 days  
**Value**: Medium - Greatly improves usability

**Technical Approach**:
- Implement `HistoryManager` class with command storage
- Add history navigation (up/down arrows)
- Implement `!n` syntax for command recall

### 6.4 Matrix Inversion
**Implementation Time**: 2-3 days
**Value**: Medium - Shows advanced linear algebra

**Technical Approach**:
- Implement Gaussian elimination with pivoting
- Handle singular matrices gracefully
- Add `inv()` function to expression parser

### 6.5 Extended Mathematical Functions
**Implementation Time**: 2-3 days
**Value**: High - Makes the calculator much more capable

**Technical Approach**:
- Extend `CustomMath` class with trigonometric functions
- Implement Taylor series for transcendental functions
- Add function name recognition to parser

## Development Timeline Summary

**Total Estimated Time**: 15-18 working days
- **Mandatory features**: 11-13 days
- **Selected bonus features**: 4-5 days
- **Buffer for testing/polish**: Built into each phase

**Critical Path**:
1. Foundation & Types (3 days)
2. Complex & Matrix support (5 days)  
3. Functions & Equations (3 days)
4. Cross-type operations (2 days)
5. Selected bonus features (4 days)
6. Final polish (1 day)

This plan balances ambitious functionality with realistic timelines, reuses your excellent computor v1 foundation, and ensures a polished final product that demonstrates advanced programming and mathematical concepts.
