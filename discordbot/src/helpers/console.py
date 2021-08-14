from rich.console import Console


class MASZConsole(Console):
    def __init__(self):            
        super().__init__()
        
    def verbose(self, string: str):
        self.log(f"[bright_black][V][/bright_black] {string}")

    def info(self, string: str):
        self.log(f"[bright_black][[white]I[/white]][/bright_black] {string}")

    def critical(self, string: str):
        self.log(f"[bright_black][[bright_red]C[/bright_red]][/bright_black] {string}")

console = MASZConsole()
