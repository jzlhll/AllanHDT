﻿using System.Linq;

namespace HearthMirror.Mono
{
	internal class MonoClass
	{
		private readonly uint _pClass;
		private readonly ProcessView _view;

		public MonoClass(ProcessView view, uint pClass)
		{
			_view = view;
			_pClass = pClass;
		}

		public string Name => _view.ReadCString(_view.ReadUint(_pClass + Offsets.MonoClass_name));

		public string NameSpace => _view.ReadCString(_view.ReadUint(_pClass + Offsets.MonoClass_name_space));

		public string FullName
		{
			get
			{
				var name = Name;
				var ns = NameSpace;
				var nestedIn = NestedIn;
				while(nestedIn != null)
				{
					name = nestedIn.Name + "+" + name;
					ns = nestedIn.NameSpace;
					nestedIn = nestedIn.NestedIn;
				}
				return ns.Length == 0 ? name : ns + "." + name;
			}
		}

		public uint VTable
		{
			get
			{
				var rti = _view.ReadUint(_pClass + Offsets.MonoClass_runtime_info);
				return _view.ReadUint(rti + Offsets.MonoClassRuntimeInfo_domain_vtables);
			}
		}

		public bool IsValueType => 0 != (_view.ReadUint(_pClass + Offsets.MonoClass_bitfields) & 8);

		public bool IsEnum => 0 != (_view.ReadUint(_pClass + Offsets.MonoClass_bitfields) & 0x10);

		public int Size => _view.ReadInt(_pClass + Offsets.MonoClass_sizes);

		public MonoClass Parent
		{
			get
			{
				var pParent = _view.ReadUint(_pClass + Offsets.MonoClass_parent);
				return pParent == 0 ? null : new MonoClass(_view, pParent);
			}
		}

		public MonoClass NestedIn
		{
			get
			{
				var pNestedIn = _view.ReadUint(_pClass + Offsets.MonoClass_nested_in);
				return pNestedIn == 0 ? null : new MonoClass(_view, pNestedIn);
			}
		}

		public MonoType ByvalArg => new MonoType(_view, _pClass + Offsets.MonoClass_byval_arg);

		public int NumFields => _view.ReadInt(_pClass + Offsets.MonoClass_field_count);

		public MonoClassField[] Fields
		{
			get
			{
				var nFields = NumFields;
				var nFieldsParent = Parent?.NumFields ?? 0;
				var pFields = _view.ReadUint(_pClass + Offsets.MonoClass_fields);
				var fs = new MonoClassField[nFields + nFieldsParent];
				for(var i = 0; i < nFields; i++)
					fs[i] = new MonoClassField(_view, pFields + (uint) i*Offsets.MonoClassField_sizeof);
				for(var i = 0; i < nFieldsParent; i++)
					fs[nFields + i] = Parent?.Fields[i];
				return fs;
			}
		}

		public dynamic this[string key] => Fields.FirstOrDefault(x => x.Name == key)?.StaticValue;
	}
}
