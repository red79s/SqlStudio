using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneratorLib
{
    public class ArrayInst
    {
        private Dictionary<string, string> _variables = new Dictionary<string, string>();
        private Dictionary<string, List<ArrayInst>> _arrays = new Dictionary<string, List<ArrayInst>>();
        private bool _bWriteSeperatorOnFirstElement = false;

        public void Clear()
        {
            _variables.Clear();
            _arrays.Clear();
        }

        public bool WriteSeperatorOnFirstElement
        {
            get { return _bWriteSeperatorOnFirstElement; }
            set { _bWriteSeperatorOnFirstElement = value; }
        }

        public void AddVariable(string name, string value)
        {
            _variables[name] = value;
        }

        public string GetVariable(string name)
        {
            if (_variables.ContainsKey(name))
                return _variables[name];
            return null;
        }

        public ArrayInst AddArrayInst(string name)
        {
            ArrayInst arrayInst = new ArrayInst();
            if (!_arrays.ContainsKey(name))
            {
                List<ArrayInst> array = new List<ArrayInst>();
                _arrays[name] = array;
            }
            _arrays[name].Add(arrayInst);
            return arrayInst;
        }

        public List<ArrayInst> GetArray(string name)
        {
            if (_arrays.ContainsKey(name))
                return _arrays[name];
            return null;
        }
    }

    public class CodeGeneratorInput
    {
        ArrayInst _global = null;
        
        public CodeGeneratorInput()
        {
            _global = new ArrayInst();
        }

        public void Clear()
        {
            _global.Clear();
        }

        public void AddGlobal(string name, string value)
        {
            _global.AddVariable(name, value);
        }

        public string GetGlobal(string name)
        {
            return _global.GetVariable(name);
        }

        public ArrayInst AddArrayInst(string name)
        {
            return _global.AddArrayInst(name);
        }

        public List<ArrayInst> GetArray(string name)
        {
            return _global.GetArray(name);
        }

        public List<ArrayInst> GetRoot()
        {
            List<ArrayInst> ret = new List<ArrayInst>();
            ret.Add(_global);
            return ret;
        }
    }
}
