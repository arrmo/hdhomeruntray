//---------------------------------------------------------------------------
// Copyright (c) 2021 Michael G. Brehm
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//---------------------------------------------------------------------------

#ifndef __TUNER_H_
#define __TUNER_H_
#pragma once

#pragma warning(push, 4)

using namespace System;

using namespace Newtonsoft::Json;
using namespace Newtonsoft::Json::Linq;

namespace zuki::hdhomeruntray::discovery {

//---------------------------------------------------------------------------
// Class Tuner
//
// Describes an individual HDHomeRun tuner
//---------------------------------------------------------------------------

public ref class Tuner
{
public:

	//-----------------------------------------------------------------------
	// Properties

	// Index
	//
	// Gets the resource index of the tuner instance
	property int Index
	{
		int get(void);
	}

	// IsActive
	//
	// Gets a flag indicating if the tuner is active or not
	property bool IsActive
	{
		bool get(void);
	}

internal:

	//-----------------------------------------------------------------------
	// Internal Member Functions

	// Create
	//
	// Creates a new Tuner instance
	static Tuner^ Create(JObject^ tuner);

private:

	// Instance Constructor
	//
	Tuner(JObject^ tuner);

	//-----------------------------------------------------------------------
	// Member Variables

	int				m_index = -1;			// The tuner index number
	__int64			m_frequency;			// Frequency that is tuned
	String^			m_targetip;				// Target IP address of the tuner
};

//---------------------------------------------------------------------------

} // zuki::hdhomeruntray::discovery

#pragma warning(pop)

#endif	// __TUNER_H_