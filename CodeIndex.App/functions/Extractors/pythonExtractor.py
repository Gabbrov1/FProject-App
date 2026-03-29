import ast,json,time,sys
from dataclasses import asdict
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
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            source = f.read()
        #print("line: "+source)
    except FileNotFoundError:
        print(f"File not found: {file_path}", flush=True)
        return []
    except Exception as e:
        print(f"Error reading file {file_path}: {e}", flush=True)
        return []

    #Parses Code to AST and walks through it to find function and class definitions
    tree = ast.parse(source)
    snippets = []

    #Walks through AST nodes to find function and class definitions
    print("walking...", flush=True)
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
    print (f"Extracted {len(result)} code snippets from {input_file}",flush=True)

    # Ensure the temp directory exists
    os.makedirs('./temp', exist_ok=True)    

    try:
        with open('./temp/output.json', 'w', encoding='utf-8') as f:
            json.dump([asdict(s) for s in result], f, ensure_ascii=False, indent=4)  # dump as JSON to file
        sys.exit(0)  # Exit with success code

    except FileNotFoundError:
        print("Output file path not found: ./temp/output.json", flush=True)
        sys.exit(1)  # Exit with error code

    except Exception as e:
        print(f"Error writing to output file: {e}", flush=True)
        sys.exit(1)  # Exit with error code
