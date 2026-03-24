import ast
import tokenize
import io
import json
import sys

source = sys.stdin.read()
tree = ast.parse(source)
lines = source.splitlines()

def extract_node(node):
    return {
        "type": type(node).__name__,
        "name": getattr(node, "name", None),
        "start": node.lineno,
        "end": node.end_lineno,
        "docstring": ast.get_docstring(node),
        "children": [
            extract_node(n)
            for n in getattr(node, "body", [])
            if isinstance(n, (ast.FunctionDef, ast.AsyncFunctionDef, ast.ClassDef))
        ]
    }

structure = extract_node(tree)

comments = []
tokens = tokenize.generate_tokens(io.StringIO(source).readline)
for tok in tokens:
    if tok.type == tokenize.COMMENT:
        comments.append({
            "text": tok.string,
            "line": tok.start[0]
        })

print(json.dumps({
    "structure": structure,
    "comments": comments
}))