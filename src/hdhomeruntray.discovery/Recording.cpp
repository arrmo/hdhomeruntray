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

#include "stdafx.h"

#include "Recording.h"

#pragma warning(push, 4)

namespace zuki::hdhomeruntray::discovery {

//---------------------------------------------------------------------------
// Recording Constructor (private)
//
// Arguments:
//
//	recording		- Reference to the JSON status data for the recording

Recording::Recording(JObject^ recording)
{
	if(Object::ReferenceEquals(recording, nullptr)) throw gcnew ArgumentNullException("recording");

	// The only thing in a recording is the name
	JToken^ name = recording->GetValue("Name", StringComparison::OrdinalIgnoreCase);
	m_name = (!Object::ReferenceEquals(name, nullptr)) ? name->ToObject<String^>() : String::Empty;
}

//---------------------------------------------------------------------------
// Recording::Create (internal)
//
// Creates a new Recording instance
//
// Arguments:
//
//	recording		- Reference to the JSON status data for the recording

Recording^ Recording::Create(JObject^ recording)
{
	if(Object::ReferenceEquals(recording, nullptr)) throw gcnew ArgumentNullException("recording");
	return gcnew Recording(recording);
}

//---------------------------------------------------------------------------
// Recording::Name::get
//
// Gets the name of the recording

String^ Recording::Name::get(void)
{
	return m_name;
}

//---------------------------------------------------------------------------

} // zuki::hdhomeruntray::discovery

#pragma warning(pop)
