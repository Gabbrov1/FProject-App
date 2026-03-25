import ast,json
from dataclasses import dataclass

@dataclass
class CodeSnippet:
    name: str
    kind: str        # "function" | "class" | "method"
    lineno: int
    end_lineno: int
    source: str

def extract_code_snippets(file_path: str) -> list[CodeSnippet]:
    #Opens file and reads source code. Closes file automatically after reading.
    with open(file_path, 'r', encoding='utf-8') as f:
        source = f.read()
        #print("line: "+source)
    #Parses Code to AST and walks through it to find function and class definitions
    tree = ast.parse(source)
    snippets = []

    #Walks through AST nodes to find function and class definitions
    print("walking...")
    for node in ast.walk(tree):
        if isinstance(node, (ast.FunctionDef, ast.ClassDef)):
            # sets the kind to "function" if it's a function definition, otherwise "class"
            kind = "function" if isinstance(node, ast.FunctionDef) else "class"
            #sets the end of snippet
            end_lineno = getattr(node, 'end_lineno', node.lineno)
            # Actual Source code snippet.
            snippet_source = ast.get_source_segment(source, node)

            snippets.append(CodeSnippet(
                name=node.name,
                kind=kind,
                lineno=node.lineno,
                end_lineno=end_lineno,
                source=snippet_source
            ))

    return snippets

if __name__ == "__main__":
    # argv[0] is the script name, [1] is your text
    input_file = sys.argv[1]
    result = extract_code_snippets(input_file)
    print (f"Extracted {len(result)} code snippets from {input_file}")
    time.sleep(100)  # Sleep for a moment to ensure all output is printed before writing to file
    with open('temp/output.json', 'w', encoding='utf-8') as f:
        json.dump([asdict(s) for s in result], f, ensure_ascii=False, indent=4)  # dump as JSON to file
