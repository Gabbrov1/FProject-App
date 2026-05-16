import ast, json, os, sys
from dataclasses import asdict, dataclass

@dataclass
class CodeSnippet:
    name: str
    kind: str
    lineno: int
    end_lineno: int
    source: str

def extract_code_snippets(file_path: str) -> list[CodeSnippet]:
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            source = f.read()
    except FileNotFoundError:
        print(f"File not found: {file_path}", flush=True)
        return []
    except Exception as e:
        print(f"Error reading file {file_path}: {e}", flush=True)
        return []

    tree = ast.parse(source)
    snippets = []

    print("walking...", flush=True)
    for node in ast.walk(tree):
        if isinstance(node, (ast.FunctionDef, ast.ClassDef)):
            kind = "function" if isinstance(node, ast.FunctionDef) else "class"
            end_lineno = getattr(node, 'end_lineno', node.lineno)
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
    input_file = sys.argv[1]
    filename = os.path.basename(input_file)
    result = extract_code_snippets(input_file)
    print(f"Extracted {len(result)} code snippets from {input_file}", flush=True)

    os.makedirs('./temp', exist_ok=True)

    output = {
        filename: {
            "lang": "python",
            "Content": [asdict(s) for s in result]
        }
    }

    try:
        with open('./temp/output.json', 'w', encoding='utf-8') as f:
            json.dump(output, f, ensure_ascii=False, indent=4)
        sys.exit(0)

    except FileNotFoundError:
        print("Output file path not found: ./temp/output.json", flush=True)
        sys.exit(1)

    except Exception as e:
        print(f"Error writing to output file: {e}", flush=True)
        sys.exit(1)