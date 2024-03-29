using System;
using System.Text;
using System.Collections;

// C# Translation of GoldParser, by Marcus Klimstra <klimstra@home.nl>.
// Based on GOLDParser by Devin Cook <http://www.devincook.com/goldparser>.
namespace GoldParser
{
	/// Each state in the Determinstic Finite Automata contains multiple edges which
	/// link to other states in the automata. This class is used to represent an edge.
	internal class FAEdge
	{
		private String	m_characters;
		private int		m_targetIndex;
		
		/* constructor */

		public FAEdge(String p_characters, int p_targetIndex)
		{
			m_characters = p_characters;
			m_targetIndex = p_targetIndex;
		}
		
		/* properties */

		public String Characters
		{
			get { return m_characters; }
			set { m_characters = value; }
		}

		public int TargetIndex
		{
			get { return m_targetIndex; }
			set { m_targetIndex = value; }
		}
		
		/* public methods */

		public void AddCharacters(String p_characters)
		{
			m_characters = m_characters + p_characters;
		}

		public override String ToString()
		{
			return "DFA edge [chars=[" + m_characters + "],action=" + m_targetIndex + "]";
		}
	}
	
	/// Represents a state in the Deterministic Finite Automata which is used by
	/// the tokenizer.
	internal class FAState
	{
		private ArrayList	m_edges;
		private int			m_acceptSymbol;
		
		/* constructor */
		
		public FAState()
		{
			m_edges = new ArrayList();
			m_acceptSymbol = -1;
		}
		
		/* properties */

		public ArrayList Edges
		{
			get { return m_edges; }
		}

		public int AcceptSymbol
		{
			get { return m_acceptSymbol; }
			set { m_acceptSymbol = value; }
		}

		public int EdgeCount
		{
			get { return m_edges.Count; }
		}

		/* public methods */
		
		public FAEdge GetEdge(int p_index)
		{
			if (p_index >= 0 && p_index < m_edges.Count)
				return (FAEdge)m_edges[p_index];
			else
				return null;
		}

		public void AddEdge(String p_characters, int p_targetIndex)
		{
			if (p_characters.Equals(""))
			{
				FAEdge edge = new FAEdge(p_characters, p_targetIndex);
				m_edges.Add(edge);
			}
			else
			{
				int index = -1;
				int edgeCount = m_edges.Count;
				
				// find the edge with the specified index
				for (int n = 0; (n < edgeCount) && (index == -1); n++)
				{
					FAEdge edge = (FAEdge)m_edges[n];
					if (edge.TargetIndex == p_targetIndex)
						index = n;
				}
				
				// if not found, create a new edge
				if (index == -1)
				{
					FAEdge edge = new FAEdge(p_characters, p_targetIndex);
					m_edges.Add(edge);
				}
				// else add the characters to the existing edge
				else
				{
					FAEdge edge = (FAEdge)m_edges[index];
					edge.AddCharacters(p_characters);
				}
			}
		}

		public override String ToString()
		{
			StringBuilder result = new StringBuilder();
			result.Append("DFA state:\n");
			
			foreach (FAEdge edge in m_edges)
			{
				result.Append("- ").Append(edge).Append("\n");
			}
			
			if (m_acceptSymbol != -1)
				result.Append("- accept symbol: ").Append(m_acceptSymbol).Append("\n");
			
			return result.ToString();
		}
	}
}
