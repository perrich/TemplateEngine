TemplateEngine
==============

A Template engine which allow to replace and hide some text (using boolean expression evaluator)

How to use it ?
---

- create a tag used by the replacement behavior {tag=xxxx}
- create a conditional part using boolean expression {if="condition"}...{/if}. A condition can manage multiple boolean variable, constant (true/false) and 3 boolean operator (not "!", and "&&" and or "||")

Sample :
```csharp
var dict = new Dictionary<string, bool> { { "Displayed", true } };
var replacements = new Dictionary<string, string> { { "value", "very small text" } };
var evaluator = new ExpressionEvaluator(dict, false);

var engine = new TemplateEngine.TemplateEngine(evaluator, false, false, replacements);
string result = engine.Apply(@"It's a sample with {if=""Displayed || true""}a displayed {tag=value}.{/if}{if=""!Displayed""}nothing.{/if}");
```

Used library:
---
- NUnit 2.6.3 for unit tests


License:
---
Copyright 2013 PERRICHOT Florian

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
