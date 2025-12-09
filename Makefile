# -=-=-=-=-    COLOURS -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

DEF_COLOR   = \033[0;39m
YELLOW      = \033[0;93m
CYAN        = \033[0;96m
GREEN       = \033[0;92m
BLUE        = \033[0;94m
RED         = \033[0;91m

# -=-=-=-=-    NAME -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= #

NAME        := computorV2
PROJECT     := computorV2
TESTS		:= ComputorV2.Tests

# -=-=-=-=-    DOTNET SETTINGS -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

DOTNET      = dotnet
BUILD_FLAGS = --configuration Release --verbosity quiet --no-restore
RUN_FLAGS   = --no-build --configuration Release

# -=-=-=-=-    PATH -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

RM          = rm -fr
BUILD_DIR   = bin
OBJ_DIR     = obj

# -=-=-=-=-    FILES -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

CSPROJ      = $(PROJECT).csproj
SOURCES     = 	Program.cs							\
				Core/Math/CustomMath.cs				\
				Core/Math/MathEvaluator.cs	\
				Core/Types/RationalNumber.cs		\
				Core/Parsing/Parser.cs				\
				Core/Types/Polynomial.cs			\
				IO/DisplayManager.cs				\
				IO/InputHandler.cs					\
				Interactive/HelpSystem.cs			\
				Interactive/HistoryManager.cs		\
				Interactive/REPL.cs					\
				Core/Lexing/Tokenizer.cs			\

EXECUTABLE  = $(BUILD_DIR)/Release/net8.0/$(PROJECT)
DLL_FILE    = $(BUILD_DIR)/Release/net8.0/$(PROJECT).dll
BUILD_MARKER = $(BUILD_DIR)/.build_marker

DEPS        = $(CSPROJ) $(SOURCES) Makefile

# -=-=-=-=-    TARGETS -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

all: $(NAME)

$(BUILD_MARKER): $(DEPS)
	@mkdir -p $(BUILD_DIR)
	@$(DOTNET) restore --verbosity quiet
	@$(DOTNET) build $(BUILD_FLAGS)
	@touch $(BUILD_MARKER)
	@echo "$(GREEN)Build completed$(DEF_COLOR)"

$(NAME): $(BUILD_MARKER)
	@if [ -f "$(DLL_FILE)" ]; then \
		if [ ! -L "$(NAME)" ] || [ ! -e "$(NAME)" ] || [ "$(BUILD_MARKER)" -nt "$(NAME)" ]; then \
			echo "$(CYAN)Creating executable link...$(DEF_COLOR)"; \
			echo "#!/bin/bash" > $(NAME); \
			echo "cd \"\$$(dirname \"\$$0\")\"" >> $(NAME); \
			echo "exec $(DOTNET) \"$(DLL_FILE)\" \"\$$@\"" >> $(NAME); \
			chmod +x $(NAME); \
		fi; \
	else \
		echo "$(RED)Build failed - DLL not found$(DEF_COLOR)"; \
		exit 1; \
	fi
	@echo "$(RED)This isn't even my final polynomial form$(DEF_COLOR)"

check-build: $(BUILD_MARKER)
	@echo "$(GREEN)Build is up to date$(DEF_COLOR)"

build: $(NAME)

run: $(BUILD_MARKER)
	@echo "$(CYAN)Running $(NAME)...$(DEF_COLOR)"
	@$(DOTNET) run $(RUN_FLAGS)

test:
	@echo "$(CYAN)Running $(NAME) tests...$(DEF_COLOR)"
	@$(DOTNET) test

clean:
	@echo "$(YELLOW)Cleaning build artifacts...$(DEF_COLOR)"
	@$(DOTNET) clean --verbosity quiet 2>/dev/null || true
	@$(RM) $(BUILD_DIR) $(OBJ_DIR) $(BUILD_MARKER)
	@echo "$(RED)Cleaned object files and build artifacts$(DEF_COLOR)"
	@echo "$(YELLOW)Cleaning test artifacts...$(DEF_COLOR)"
	@cd ComputorV2.Tests && $(DOTNET) clean --verbosity quiet 2>/dev/null || true
	@$(RM) ComputorV2.Tests/bin ComputorV2.Tests/obj
	@echo "$(RED)Cleaned test related files$(DEF_COLOR)"

fclean: clean
	@$(RM) $(NAME)
	@echo "$(RED)Cleaned all binaries$(DEF_COLOR)"

re: fclean all

.PHONY: all build run test publish restore clean fclean re force dry-run check-build info help

# -=-=-=-=-    DEPENDENCY RULES -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

.PRECIOUS: $(BUILD_MARKER)

$(BUILD_MARKER): $(SOURCES) $(CSPROJ) Makefile