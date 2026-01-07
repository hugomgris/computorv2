# <h1 align="center">COMPUTORV2</h1>

<p align="center">
   <b>A comprehensive, interactive interpreter algebra interpreter and calculator built in C#, featuring computational mathematics management between different types (Rational and Complex numbers, Matrices, Polynomial Functions), with built in graphical representation, variable storing and equation handling.</b><br>
</p>

---

<p align="center">
    <img alt="C#" src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white" />
    <img alt=".NET" src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white" />
    <img alt="Math" src="https://img.shields.io/badge/Mathematics-FF6B6B?style=for-the-badge&logo=mathworks&logoColor=white" />
    <img alt="Polynomial Algebra" src="https://img.shields.io/badge/Polynomial%20Algebra-4ECDC4?style=for-the-badge" />
</p>

## Table of Contents
1. [Project Overview](#project-overview)
2. [Feature Breakdown](#feature-breakdown)
3. [Execution Instructions](#execution-instructions)

---

## Project overview
This project aims to build a fairly complex, yet easy to build mathematics interpreter, extending the functionality from a [previous version](https://https://github.com/hugomgris/computorv1) centered around polynomial solving, adding a layer of interactivity on top and history management. The extended features include, among other functionalities, variable assignation and store, expression computation with Rational Number, Complex Number, Matrix and Polynomial type handling, graphical representation of functions and a comprehensive REPL pipeline built on top a postfix conversor (based on the Shunting Yard algorithm) to allow cross-type computation. It also contains several QOL elements, such as storage check commands, history management commands and a help option with basic instructions.

## Feature Breakdown

Besides the inherited functionalities from the `V1` version, which are compiled in [its own readme file](https://github.com/hugomgris/computorv1/blob/main/README.md), `V2` features:

### Math Value Parsing, Tokenization and Handling
Parsing the input in `V2` is a considerably more complex endevour than in `V1`. Because of the increased functionality and the need for multi type handling, the tokenization of the input data needs to juggle quite a lot of edge cases, all while being able to detect key, type-dependent caes. For example, an input of `3*-1=?`, which calls for a computation of 3 times -1, needs to parse the `-` symbol as a unary minus, instead of an operator, so that the input is not rejected as an invalid consecutive operator case. Same goes for, to take another example, complex numbers, as `5+3i` needs to be parsed as a whole token, a complex number with a real value of 5 and an imaginary value of 3, all while avoiding the possible detection of the `+` token as an operator.

Building a strong parsing and tokenizing class (or, in this cases, classes) was the first keystone in the project, as without being able to assemble a correct infix token inflow there was no point in attempting to set up a computing pipeline. Besides the core classes that contain the interaction and CLI related functionality (the REPL, the History Manager, the Result Output Manager), the Tokenizer class is, alongiside the Math Evaluator, the core of ComputorV2's engine. All calls for computation, be they a direct call or a necessary step for an assignment call, need to go through tokenization before stepping into the infix->postfix transformation.

### REPL and CLI interactive loop
The task asked for an "interpreter that, like a shell, retrieves user inputs for advanced computations", i.e. a continuous loop of reading and processing input, computing and building output. This was tackled with an REPL structure in mind (Read-Eval-Print Loop) that works in steps:
- User entres an expression that goes through a parsing process for basic syntax errors (double `=` tokens, continious operators (`+-*/^%` with beforementioned edge cases), broken or incomplete inputs, ...)
- Input expression is checked for builtin commands, such as `exit`, `clearhistory`, `help`, `listvariables`, etc.
- If input is asking for a non builtin command call, be it an assignation or a computation, the received string goes through command detection and processing and sent to the necessary pipeline inside the Math Evaluator.
- After the math evaluation process, which results in a string result (some error cases and throws can happen midway, but generally speaking the input string results in an output string), said result is printed in console, the command is stored in history and the loop goes back to the beginning, printing the prompt (nothing fancy, a simple `> `) and the processing begins again.

For the correct functioning of this REPL approach, the interaction of the console needed to be coded too, for which an IO class was made. Through it, the keyhooks for CLI navigation (arrow keys, home and end) where set up so that the user could have cursor control (left-right) and history navigation (up-down). Paired up with a basic readline pipeline, a basic console based .NET application was achieved.

### Persistent History
During execution, a string based list stores non-error inputs. This list is stored in a history file (by default, inside the /docs directory, as an invisible `.history` document) he contents of which, if any, are also loaded when the application starts (and if that is the case, the ammount of loaded commands is displayed in the terminal, below the welcome banner). At any point during execution, users can input `clearhistory` or `cl` as history wiping commands, which will delete all contents of the runtime list-based history, which itself will result in a wipe of the contents of the saved file.

### Comprehensive Math Evaluator
There are two main execution pipelines in this project. The first one is the `Assignment` pipeline, which allows the user to store any supported type value in named variables (names have some restrictions: only alphabetical characters, no `i` char allowed for Complex Number reasons). The second one is the `Computation` pipeline, which allows users to compute directly input expression (e.g., `4 - 3 - ( 2 * 3 ) ^ 2 * ( 2 - 4 ) + 4 = ?`), call for a computation of an expression including previously stored variables (e.g, `varA = 42`, `varA * 1.67 = ?`), and retrieval of stored values (e.g., `varA=?` will return the value assigned to `varA` if it exists, will throw an error if it doesn't).

Both of this pathways are built around a handful of custom class-based types, all inheriting from a master `MathValue` abstract class that works as a centrilizing, all-calculation-purpose type. By this I mean that, in a logical C# way, all the computation management works by passing and returning `MathValue` types, and then the computation itself is done via the net of overrides that each class implements for the supported mathematical operations (Addition, Substraction, Multiplication, Division, Modulo, Power). This was the best way I could come up with to make the math engine core piece work, the infix-to-postfix generator, amidst the tides of both attempting to write this calculator while deepening my C# knowledge.

In fewer words, after the input is parsed as either an `Assignment` or a `Computation` call, and after passing basic syntax checks, the input is tokenized, it's contained types and operators are detected and the whole thing is sent to the postfixer.

### Postfixer
This was the hardest, most important piece of the puzzle. ComputorV2 is a calculator based on string input, so a way of translating that input into a manageable mathematical expression that can be then, well, calculated required some research on my part. I ended up choosing an infix->postfix approach based on the Shunting Yard algorithm. Specifically, the input expression, treated as a typical `infix expression` (i.e., how we, humans, usually write arithmetical expressions, `3 + 4" or "3 + 4 × (2 − 1)`) is converted via the postfixer class into an `postfix expression` (from the previous example, `3 4 2 1 − × +`) to build what is called an Reverse Polish Notation expression (or RPN; also: I'm using a lot the word expression here, sorry). With that on the table, calculations are easily managed with a double stack based solver of RPN notations. Steps are not too complex, to be honest:
- Tokenize input
- Clean tokens and build the infix version of the input
- Make the postfix list from the infix one (a couple of annoying steps here, all related to the Shunting Yard algo, like operator precedence checks, associativeness management, parenthesis handling, ... Nothing too sofisticated, but that sadly doesn't write itself).
- Calculate the result of the postfix expression and return its parsing into string so that the REPL can print it back to the user.

The fundamental aspect in all this postfixing stuff is that it needed the previous point's implementations to work correctly across the project's type spectrum. While calculating the result of a built RPN, different types can be crossed, such as, for example, attempting to multiply a Complex Number by a Rational Number. Making the postfixer work around the base `MathValue` type and just checking for the specific types of each retrieved value of the calculation stacks when needed, so that the correct functions calls (and pitfalls) can be detected was a bit difficult to achieve, but saved me from rivers of tears if I hadn't done it this way.

### Special features
Some extended functionality was added after the core of the program was built. Appart from the possibility to check stored variables, functions and commands, graphical representation of functions was added to the `Polynomial` class, although this feature was already introduced in the `V1` version of the project. `Matrix Inverse` and `Matrix Norm` computation of pre-stored matrices was also added, rehashing the C++ code I wrote for another project ([Matrix](https://https://github.com/hugomgris/matrix)). The ComputorV2 versions of this processes could be extended and refined, but I feel that I already did that in that past project, so it is what it is, take it or leave it, etc.

Here's an example of a graphical representation of the function `f(x) = x^2 + 2x + 5`:

```
======================================================================
 137.1 |                              |                             
 129.5 |                              |                             
 121.8 |                              |                            *
 114.2 |                              |                          ** 
 106.5 |                              |                         *   
  98.9 |                              |                        *    
  91.3 |                              |                       *     
  83.6 |*                             |                      *      
  76.0 | **                           |                    **       
  68.3 |   *                          |                   *         
  60.7 |    **                        |                  *          
  53.1 |      *                       |                **           
  45.4 |       **                     |              **             
  37.8 |         **                   |            **               
  30.1 |           **                 |          **                 
  22.5 |             ***              |       ***                   
  14.8 |                ***           |    ***                      
   7.2 |                   ****************                         
  -0.4 |------------------------------+-----------------------------
  -8.1 |                              |                             
            -10.0      -6.6      -3.2       0.2       3.6       6.9
======================================================================
```

Here, an example of a graphical representation of the functin `f(x) = 42 * x + 42`:

```
======================================================================
 546.0 |                              |                             
 492.9 |                              |                             
 439.9 |                              |                         ****
 386.8 |                              |                     ****    
 333.8 |                              |                  ***        
 280.7 |                              |              ****           
 227.7 |                              |          ****               
 174.6 |                              |      ****                   
 121.6 |                              |   ***                       
  68.5 |                              ****                          
  15.5 |--------------------------****+-----------------------------
 -37.6 |                       ***    |                             
 -90.6 |                   ****       |                             
-143.7 |               ****           |                             
-196.7 |           ****               |                             
-249.8 |        ***                   |                             
-302.8 |    ****                      |                             
-355.9 |****                          |                             
-408.9 |                              |                             
-462.0 |                              |                             
            -10.0      -6.6      -3.2       0.2       3.6       6.9
======================================================================
```

My personal thoughts: it is kinda cool.

## Execution Instructions
Pretty straight forward:
- Clone the repo
- Run `make run` command in your console application
- Enjoy some mathematical fun

If the build process fails, modify the `DOTNET` path inside the Makefile so that it points to wherever you have .NET installed in your system.

---
Thanks for reading, any feedback is welcomed.
