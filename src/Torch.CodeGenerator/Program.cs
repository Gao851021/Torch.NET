﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Torch.CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Declarations.yaml excerpt
            string declarations_yaml = @"
- name: empty
  matches_jit_signature: true
  schema_string: aten::empty(int[] size, *, ScalarType? dtype=None, Layout? layout=None, Device? device=None, bool? pin_memory=None) -> Tensor
  method_prefix_derived: ''
  arguments:
  - annotation: null
    dynamic_type: IntArrayRef
    is_nullable: false
    name: size
    type: IntArrayRef
  - annotation: null
    default: '{}'
    dynamic_type: TensorOptions
    is_nullable: false
    kwarg_only: true
    name: options
    type: const TensorOptions &
  method_of:
  - Type
  - namespace
  mode: native
  python_module: ''
  returns:
  - dynamic_type: Tensor
    name: result
    type: Tensor
  inplace: false
  is_factory_method: false
  abstract: true
  requires_tensor: false
  device_guard: true
  with_gil: false
  deprecated: false
- name: _empty_affine_quantized
  matches_jit_signature: true
  schema_string: aten::_empty_affine_quantized(int[] size, *, ScalarType? dtype=None, Layout? layout=None, Device? device=None, bool? pin_memory=None, float scale=1, int zero_point=0) -> Tensor
  method_prefix_derived: ''
  arguments:
  - annotation: null
    dynamic_type: IntArrayRef
    is_nullable: false
    name: size
    type: IntArrayRef
  - annotation: null
    default: '{}'
    dynamic_type: TensorOptions
    is_nullable: false
    kwarg_only: true
    name: options
    type: const TensorOptions &
  - annotation: null
    default: 1
    dynamic_type: double
    is_nullable: false
    kwarg_only: true
    name: scale
    type: double
  - annotation: null
    default: 0
    dynamic_type: int64_t
    is_nullable: false
    kwarg_only: true
    name: zero_point
    type: int64_t
  method_of:
  - Type
  - namespace
  mode: native
  python_module: ''
  returns:
  - dynamic_type: Tensor
    name: result
    type: Tensor
  inplace: false
  is_factory_method: false
  abstract: true
  requires_tensor: false
  device_guard: true
  with_gil: false
  deprecated: false
";
            #endregion

            /* for test, generate this piece of code for torch.empty api.
             public Tensor empty(params int[] dims)
            {
                var tuple = new PyTuple(dims.Select(x => new PyInt(x)).ToArray());
                dynamic py = torch.empty(tuple);

                return new Tensor(py.Handle);
            } */


            var input = new StringReader(declarations_yaml);
            var deserializer = new DeserializerBuilder()
                //.WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            var declarations = deserializer.Deserialize<List<Declaration>>(input);
            var s = new StringBuilder();
            var generator=new CodeGenerator();
            foreach (var decl in declarations)
            {
                Console.WriteLine(decl.schema_string);
                generator.GenerateApiFunction(decl, s);
            }
            Console.WriteLine("Generated code:\r\n");
            Console.WriteLine(s);
            Console.ReadKey();
        }
    }
}